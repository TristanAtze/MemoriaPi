using System.Diagnostics;
using System.Security; // Für SecureString
using System.Text;
using MemoriaPiCrypo.Cypher;

namespace MemoriaPiCrypto;

// Wichtiger Hinweis: SecureString und das sichere Löschen von Speicher sind komplexe Themen.
// SecureString hat Einschränkungen, und 100% garantierte sichere Speicherlöschung kann schwierig sein.
// Die hier gezeigten Methoden folgen gängigen Praktiken, aber es ist kein absoluter Schutz gegen alle Arten von Angriffen.

/// <summary>
/// Bietet ECHTE sichere Ver- und Entschlüsselung mit Authentifizierung und Integrität
/// mittels AES-256 im GCM-Modus. Nutzt standardmäßige, kryptographisch geprüfte .NET Klassen.
/// Erweitert um Versionierung, sehr hohe PBKDF2-Iterationen und Unterstützung für Associated Data (AAD).
/// Enthält die gemeinsame Hilfsmethode zum sicheren Löschen von Bytes.
/// </summary>
/// <remarks>
/// Implementiert Versionierung des Chiffretext-Formats.
/// Nutzt PBKDF2 für sichere Schlüsselableitung mit SEHR hohem Iterations-Count.
/// Speichert Version, Salt, Nonce (IV) und Authentifizierungs-Tag zusammen mit dem Chiffretext.
/// Ermöglicht die Einbindung zusätzlicher, nicht-verschlüsselter, aber authentifizierter Daten (AAD).
/// Bietet robusten Schutz vor Manipulation.
/// DIE SICHERE VERWALTUNG DES SCHLÜSSELS/PASSWORTS IST ENTSCHEIDEND UND LIEGT AUSSERHALB DIESES CODES!
/// </remarks>


/// <summary>
/// Kapselt P/Invoke-Aufrufe an das Windows Credential Manager API für sichere Passwortspeicherung.
/// Diese Klasse ist WINDOWS-SPEZIFISCH.
/// </summary>


// === Hauptprogrammlogik der Crypto ===
public static class Crypto
{
    private static byte[] myAssociatedData = Encoding.UTF8.GetBytes("System Configuration");
    private static string KeyTxt = "Key.txt";

    static Crypto()
    {
        FirstCallOnly();
    }

    public static void FirstCallOnly()
    {
        if (!File.Exists(KeyTxt))
        {
            File.WriteAllText("Key.txt", ConvertTxtToBinary("Highly Secure Key"));
        }
    }
    
    public static string ReadBinaryFileAsText(string binaryFilePath = "Key.txt")
    {
        if (!File.Exists(binaryFilePath))
        {
            return null;
        }

        try
        {
            byte[] fileBytes = File.ReadAllBytes(binaryFilePath);

            string textContent = Encoding.UTF8.GetString(fileBytes);

            return textContent;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string ConvertTxtToBinary(string textContent)
    {
        string binaryText = "";
        try
        {
            byte[] fileBytes = Encoding.UTF8.GetBytes(textContent);
            foreach (var e in fileBytes)
            {
                binaryText += e.ToString("x8");
            }

            return binaryText;
        }
        catch (Exception)
        {
            Debug.Write("Error converting text to binary");
        }

        return "";
    }

    public static string Encryption(string plainText, string? password = null)
    {
        SecureString loadedSecurePassword = new();
        string encryptedData = "";

        if (password == null)
        {
            string passwordFromFile = ReadBinaryFileAsText("Key.txt");
            foreach (char c in passwordFromFile ?? "")
            {
                loadedSecurePassword.AppendChar(c);
            }
        }
        else
        {
            loadedSecurePassword = new();
            foreach (char c in password ?? "")
            {
                loadedSecurePassword.AppendChar(c);
            }
        }

        if (loadedSecurePassword != null)
        {
            // WICHTIG: Das geladene SecureString SOFORT für die Schlüsselableitung nutzen
            // und dann das SecureString Objekt und alle temporären Klartextkopien löschen.
            string? passwordStringForDerivation = null;

            try
            {
                // Umwandlung SecureString -> String. Dieser String ist eine temporäre Klartext-Kopie!
                passwordStringForDerivation = new System.Net.NetworkCredential("", loadedSecurePassword).Password;

                // SecureString Objekt sofort nach der Umwandlung löschen
                loadedSecurePassword.Dispose();

                // Schlüssel aus dem temporären String ableiten
                // (Die DeriveKeyFromPassword Methode ist jetzt in HighlySecureAuthenticatedVersionedCipher,
                // aber wir brauchen hier das Salt vom Encrypt/Decrypt Ergebnis, um sie direkt aufzurufen.
                // Einfacher ist, den String an Encrypt/Decrypt zu übergeben.)

                encryptedData = HighlySecureAuthenticatedVersionedCipher.Encrypt(plainText, passwordStringForDerivation, myAssociatedData) ?? "";
            }
            finally
            {
                // Sorge dafür, dass der temporäre Klartext-String aus der ersten Ableitung gelöscht wird
                // (Obwohl String.Clear nicht existiert, kann man die Referenz null setzen und auf GC hoffen)
                passwordStringForDerivation = null;
                // Abgeleiteten Schlüssel löschen, falls er hier direkt abgeleitet worden wäre
                // (In dieser Demo leitet der Cipher ihn intern ab)
                // ClearBytes(keyBytes); // Nicht notwendig in dieser Demo, da Cipher es intern tut
            }
        }

        return encryptedData;
    }

    public static async Task<string> DecryptionAsync(string encryptedData, string? password = null)
    {
        SecureString? loadedSecurePassword = new();

        if (password == null)
        {
            string passwordFromFile = ReadBinaryFileAsText("Key.txt");
            foreach (char c in passwordFromFile ?? "")
            {
                loadedSecurePassword.AppendChar(c);
            }
        }
        else if (password != null)
        {
            foreach (char c in password)
            {
                loadedSecurePassword.AppendChar(c);
            }
        }

        string decryptedData = "";
        if (loadedSecurePassword != null)
        {
            // --- Schritt 3: Geladenes Passwort für Entschlüsselung nutzen ---
            string? passwordStringForDecryption = null;
            try
            {
                passwordStringForDecryption = new System.Net.NetworkCredential("", loadedSecurePassword).Password;

                decryptedData =
                    HighlySecureAuthenticatedVersionedCipher.Decrypt(encryptedData, passwordStringForDecryption,
                        myAssociatedData) ?? "";
            }
            finally
            {
                // Lösche die SecureString Kopie und den temporären String
                loadedSecurePassword.Dispose();
                // Der string passwordStringForDecryption wird vom GC aufgeräumt
            }
        }

        return Task.FromResult(decryptedData).Result;
    }
}