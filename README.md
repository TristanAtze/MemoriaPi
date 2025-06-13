# 🛡️ MemoriaPi

> Dein privater, digitaler Hub für die Familie. MemoriaPi bietet sichere Benutzerverwaltung, Cloud-Speicher und zukünftig einen KI-Assistenten – alles auf deinem eigenen Raspberry Pi.

[![Lizenz](https://img.shields.io/badge/Lizenz-MIT-blue.svg)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Python](https://img.shields.io/badge/Python-3.x-informational)](https://www.python.org/)

MemoriaPi ist eine multifunktionale Webanwendung, die für das Self-Hosting auf einem Raspberry Pi konzipiert wurde. Das Projekt kombiniert die Robustheit von ASP.NET Core für das Backend mit der Flexibilität von Python für KI- und Automatisierungsaufgaben. Das Ziel ist es, eine sichere, private und zentrale Anlaufstelle für Familienorganisation und -interaktion zu schaffen.

## ✨ Aktuelle Funktionen

* **🔐 Sichere Benutzerauthentifizierung:**
    * Vollständiges Registrierungs- und Anmeldesystem mit ASP.NET Core Identity.
    * Passwort zurücksetzen per E-Mail.
    * Zwei-Faktor-Authentisierung (2FA) für erhöhte Sicherheit.
    * Rollenbasiertes Berechtigungssystem (Benutzer, Administrator).

* **⚙️ Administrations-Dashboard:**
    * Eine zentrale Verwaltungsoberfläche für Administratoren.
    * Benutzerkonten anzeigen, sperren/entsperren und Rollen zuweisen.
    * Sicherheitsrelevante Aktionen wie das Erzwingen einer Passwortzurücksetzung.

* **☁️ Persönlicher Cloud-Speicher (in Entwicklung):**
    * Möglichkeit zum Hoch- und Herunterladen von Dateien.
    * Jeder Benutzer hat seinen eigenen, isolierten Speicherbereich.
    * Alle Dateien werden mit einem starken, versionierten Verschlüsselungsalgorithmus gesichert.

* **🔒 Starke Ende-zu-Ende-Verschlüsselung:**
    * Eine benutzerdefinierte `HighlySecureAuthenticatedVersionedCipher`-Klasse, die AES-GCM zur authentifizierten Verschlüsselung von Daten verwendet. Stellt sicher, dass auf dem Server gespeicherte Daten ohne den korrekten Schlüssel nicht lesbar sind.

* **🐍 Python-Integration:**
    * Grundlagen für die Ausführung von Python-Skripten aus dem .NET-Backend.
    * Beinhaltet Skripte für `SpeechToText.py` und `WebcamPresenceLock.py` als Proof-of-Concept für zukünftige KI-Funktionen.

## 🛠️ Technische Übersicht & Architektur

Das Projekt ist in mehrere logische Schichten unterteilt, um eine klare Trennung der Zuständigkeiten zu gewährleisten.

* **`MemoriaPiWeb`**: Das Hauptprojekt. Eine ASP.NET Core Razor Pages-Anwendung, die die Benutzeroberfläche, Controller und API-Endpunkte bereitstellt.
* **`MemoriaPiDataCore`**: Die Datenzugriffsschicht. Verwendet Entity Framework Core zur Kommunikation mit der Datenbank (standardmäßig SQLite). Definiert die Datenmodelle und die Struktur des lokalen Dateispeichers.
* **`MemoriaPiCrypto`**: Eine dedizierte Klassenbibliothek für alle kryptografischen Operationen. Sie kapselt die Verschlüsselungslogik, um sie wiederverwendbar und sicher zu machen.

### 🤖 Tech-Stack

* **Backend:** C# mit ASP.NET Core 8
* **Datenbank:** Entity Framework Core 8 mit SQLite (einfach konfigurierbar für andere DBs)
* **Authentifizierung:** ASP.NET Core Identity
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5, jQuery, qrcode.js
* **KI & Skripting:** Python (Anbindung über Prozessaufrufe)
* **Hosting:** Konzipiert für Linux auf einem Raspberry Pi (kann aber auf jeder Plattform laufen, die .NET unterstützt)

## 🚀 Erste Schritte & Installation

Um das Projekt lokal auszuführen, befolge diese Schritte:

### Voraussetzungen

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Python 3.x](https://www.python.org/downloads/)
* Folgende Python-Pakete (installierbar via `pip`):
    ```bash
    pip install SpeechRecognition pocketsphinx opencv-python
    ```

### Konfiguration

1.  **Repository klonen:**
    ```bash
    git clone [https://github.com/dein-benutzername/memoriapi.git](https://github.com/dein-benutzername/memoriapi.git)
    cd memoriapi
    ```
2.  **Verschlüsselungsschlüssel erstellen:**
    Erstelle eine Datei namens `Key.txt` im `MemoriaPiWeb`-Verzeichnis und füge einen starken, zufälligen Schlüssel (z.B. ein [Base64-String](https://www.base64encode.org/) mit 32 Zeichen) ein.
    **Wichtig:** Diese Datei ist in der `.gitignore` enthalten und sollte niemals eingecheckt werden!

3.  **Datenbank migrieren:**
    Führe die EF Core-Migrationen aus, um die SQLite-Datenbank zu erstellen.
    ```bash
    cd MemoriaPiWeb
    dotnet ef database update
    ```
4.  **Anwendung starten:**
    ```bash
    dotnet run
    ```
    Die Anwendung ist standardmäßig unter `https://localhost:5001` erreichbar.

## 🗺️ Zukünftige Pläne (Roadmap)

* **🤖 KI-Chatbot:** Integration eines sprachgesteuerten Assistenten zur Beantwortung von Fragen und Steuerung von Familienaufgaben.
* **✅ Aufgabenverwaltung:** Ein einfaches System für Familienmitglieder, um Aufgaben zu erstellen, zuzuweisen und zu verfolgen.
* **📷 Webcam Presence Lock:** Vollständige Implementierung der Funktion, die die Anwendung sperrt, wenn keine Person mehr vor der Webcam erkannt wird.
* **🎤 Erweiterte Sprachbefehle:** Ausbau der Speech-to-Text-Funktionen zur Steuerung der gesamten Anwendung.

## 📜 Lizenz

Dieses Projekt ist unter der **MIT-Lizenz** lizenziert. Weitere Informationen findest du in der [LICENSE.txt](LICENSE.txt)-Datei.
