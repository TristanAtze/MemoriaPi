using System.Security.Cryptography; // Für RandomNumberGenerator, etc.
using System.Text;
namespace MemoriaPiCrypo.Cypher;
public static class HighlySecureAuthenticatedVersionedCipher
{
    // Versionskontrolle für das Chiffretext-Format
    private const byte CurrentVersion = 0x01; // Erste Version des Formats: Version | Salt | Nonce | Ciphertext | Tag

    // AES-GCM benötigt einen 32-Byte (256 Bit) Schlüssel für AES-256.
    private const int AesKeySizeBytes = 32;

    // AES-GCM benötigt eine Nonce (ähnlich IV) der Grösse 12 Bytes.
    private const int AesGcmNonceSizeBytes = 12;

    // AES-GCM erzeugt einen Authentifizierungs-Tag der Grösse 16 Bytes (Standard).
    private const int AesGcmTagSizeBytes = 16;

    // Die Grösse des Salts für PBKDF2.
    private const int Pbkdf2SaltSizeBytes = 16;

    // SEHR hohe Iterationen für PBKDF2.
    private const int Pbkdf2Iterations = 50000; // Oder mehr, je nach Performance-Budget

    /// <summary>
    /// Löscht den Inhalt eines Byte-Arrays sicher, indem er mit Nullen überschrieben wird.
    /// Eine defensive Massnahme. Gemacht internal static, damit andere Klassen (z.B. Credential Manager)
    /// in derselben Assembly sie nutzen können.
    /// </summary>
    /// <param name="data">Das zu löschende Byte-Array.</param>
    internal static void ClearBytes(byte[]? data)
    {
        if (data == null) return;
        // Überschreibt den Speicher mit Nullen.
        new Span<byte>(data).Clear();
    }


    /// <summary>
    /// Leitet einen kryptographisch sicheren Schlüssel (AES-256) aus einem Passwort und einem Salt ab.
    /// Nutzt PBKDF2 mit hoher Iterationszahl.
    /// </summary>
    /// <param name="password">Das Benutzerpasswort.</param>
    /// <param name="salt">Ein zufälliges Salt.</param>
    /// <returns>Ein Byte-Array, das als AES-Schlüssel verwendet werden kann.</returns>
    private static byte[] DeriveKeyFromPassword(string password, byte[] salt)
    {
        // Wir nutzen hier SHA256 als Pseudorandom Function (PRF) für PBKDF2.
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256);
        byte[] key = pbkdf2.GetBytes(AesKeySizeBytes);
        return key; // Aufrufer muss diesen Schlüssel löschen!
    }

    /// <summary>
    /// Verschlüsselt Klartext sicher mittels AES-256 im GCM-Modus mit Authentifizierung.
    /// Generiert Salt und Nonce zufällig. Enthält Version, Salt, Nonce, Chiffretext und Tag im Ergebnis.
    /// Erlaubt die Einbindung zusätzlicher, nicht-verschlüsselter, aber authentifizierter Daten (AAD).
    /// </summary>
    /// <param name="plainText">Der zu verschlüsselnde Klartext.</param>
    /// <param name="password">Das Passwort, aus dem der Schlüssel abgeleitet wird.</param>
    /// <param name="associatedData">Zusätzliche Daten (z.B. Header), die authentifiziert, aber nicht verschlüsselt werden. Kann null sein.</param>
    /// <returns>Den Base64-kodierten Chiffretext inklusive Version, Salt, Nonce und Tag, oder null bei Fehler.</returns>
    public static string? Encrypt(string plainText, string password, byte[]? associatedData = null)
    {
        if (string.IsNullOrEmpty(password)) { Console.WriteLine("Error: Password cannot be empty for secure encryption."); return null; }
        byte[] plainBytes = (string.IsNullOrEmpty(plainText)) ? [] : Encoding.UTF8.GetBytes(plainText);

        byte[]? keyBytes = null;
        byte[]? salt = null;
        byte[]? nonceBytes = null;
        byte[]? resultBytes = null;

        try
        {
            salt = new byte[Pbkdf2SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(salt); }
            keyBytes = DeriveKeyFromPassword(password, salt);

            nonceBytes = new byte[AesGcmNonceSizeBytes];
            using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(nonceBytes); }

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tagBytes = new byte[AesGcmTagSizeBytes];

            using (var aesGcm = new AesGcm(keyBytes ?? []))
            {
                aesGcm.Encrypt(nonceBytes, plainBytes, cipherBytes, tagBytes, associatedData);
            }

            int headerLength = 1 + salt.Length + nonceBytes.Length;
            resultBytes = new byte[headerLength + cipherBytes.Length + tagBytes.Length];

            resultBytes[0] = CurrentVersion;
            Buffer.BlockCopy(salt, 0, resultBytes, 1, salt.Length);
            Buffer.BlockCopy(nonceBytes, 0, resultBytes, 1 + salt.Length, nonceBytes.Length);
            Buffer.BlockCopy(cipherBytes, 0, resultBytes, headerLength, cipherBytes.Length);
            Buffer.BlockCopy(tagBytes, 0, resultBytes, headerLength + cipherBytes.Length, tagBytes.Length);

            return Convert.ToBase64String(resultBytes);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during encryption: {ex.Message}");
            return null;
        }
        finally
        {
            // Defensive Massnahme: Lösche sensible Daten im Speicher
            ClearBytes(plainBytes);
            ClearBytes(keyBytes);
            ClearBytes(salt);
            ClearBytes(nonceBytes);
        }
    }

    /// <summary>
    /// Entschlüsselt und authentifiziert einen Base64-kodierten Chiffretext, der mit
    /// der Encrypt-Methode dieser Klasse erstellt wurde. Prüft die Integrität der Daten.
    /// Nutzt optional zusätzliche, nicht-verschlüsselte, aber authentifizierte Daten (AAD).
    /// </summary>
    /// <param name="base64CipherTextWithHeader">Der Base64-kodierte Chiffretext (inklusive Version, Salt, Nonce und Tag).</param>
    /// <param name="password">Das Passwort, aus dem der Schlüssel abgeleitet wird.</param>
    /// <param name="associatedData">Dieselbe zusätzliche Daten (z.B. Header) wie bei der Verschlüsselung. Kann null sein.</param>
    /// <returns>Der entschlüsselte Klartext, oder null bei Fehler oder Manipulationsversuch (inkl. falscher Version).</returns>
    public static string? Decrypt(string base64CipherTextWithHeader, string password, byte[]? associatedData = null)
    {
        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Error: Password cannot be empty for secure decryption.");
            return null;
        }
        if (string.IsNullOrEmpty(base64CipherTextWithHeader))
        {
            return "";
        }

        byte[]? inputBytes = null;
        byte[]? salt = null;
        byte[]? nonceBytes = null;
        byte[]? tagBytes = null;
        byte[]? cipherBytes = null;
        byte[]? keyBytes = null;
        byte[]? plainBytes = null;

        try
        {
            inputBytes = Convert.FromBase64String(base64CipherTextWithHeader);

            int headerLength = 1 + Pbkdf2SaltSizeBytes + AesGcmNonceSizeBytes;
            int minLength = headerLength + AesGcmTagSizeBytes;

            if (inputBytes.Length < minLength)
            {
                Console.WriteLine($"Error: Invalid ciphertext (too short, minimum {minLength} bytes required).");
                return null;
            }

            byte version = inputBytes[0];
            if (version != CurrentVersion)
            {
                Console.WriteLine($"Error: Unsupported ciphertext version. Expected {CurrentVersion}, found {version}.");
                return null;
            }

            salt = new byte[Pbkdf2SaltSizeBytes];
            Buffer.BlockCopy(inputBytes, 1, salt, 0, salt.Length);

            nonceBytes = new byte[AesGcmNonceSizeBytes];
            Buffer.BlockCopy(inputBytes, 1 + salt.Length, nonceBytes, 0, nonceBytes.Length);

            tagBytes = new byte[AesGcmTagSizeBytes];
            Buffer.BlockCopy(inputBytes, inputBytes.Length - tagBytes.Length, tagBytes, 0, tagBytes.Length);

            int cipherLength = inputBytes.Length - headerLength - tagBytes.Length;

            if (cipherLength < 0)
            {
                Console.WriteLine("Error: Invalid ciphertext length after extracting components.");
                return null;
            }

            if (cipherLength == 0)
            {
                return "";
            }

            cipherBytes = new byte[cipherLength];
            Buffer.BlockCopy(inputBytes, headerLength, cipherBytes, 0, cipherBytes.Length);

            keyBytes = DeriveKeyFromPassword(password, salt);

            plainBytes = new byte[cipherBytes.Length];

            using (var aesGcm = new AesGcm(keyBytes))
            {
                aesGcm.Decrypt(nonceBytes, cipherBytes, tagBytes, plainBytes, associatedData);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException) { Console.WriteLine("Error: Invalid Base64 input during decryption."); return null; }
        catch (CryptographicException ex) { Console.WriteLine($"Error: Decryption or authentication failed. Data/AAD may have been tampered with, or password/key is incorrect. ({ex.Message})"); return null; }
        catch (Exception ex) { Console.WriteLine($"Error during decryption process: {ex.Message}"); return null; }
        finally
        {
            // Defensive Massnahme: Lösche sensible Daten im Speicher nach Gebrauch
            ClearBytes(inputBytes);
            ClearBytes(salt);
            ClearBytes(nonceBytes);
            ClearBytes(tagBytes);
            ClearBytes(cipherBytes);
            ClearBytes(keyBytes);
            ClearBytes(plainBytes);
        }
    }
}