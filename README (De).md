# MicMute 2

**Version:** v2.1.2  
**Letzte Änderung:** 24.05.2026  
**Autoren:** 
- AveYo (Original, nicht mehr auf Github verfügbar)
- rjcncpt (Verbesserungen)
- reaperhammer (Core Audio-Integration & Anti-Cheat-Sicherheit)

MicMute ist ein kleines Windows-Tool, das es ermöglicht, das Mikrofon per System-Tray-Icon schnell stummzuschalten oder zu aktivieren. Es zeigt den aktuellen Mikrofonzustand (an/aus) über ein Tray-Icon an und speichert diesen Zustand in einer Konfigurationsdatei.

![mic-on-off](https://github.com/user-attachments/assets/5277a8af-3598-4b3c-a46c-df598fce5b6c)

---

## Funktionen
- **System-Tray-Icon**: Zeigt an, ob das Mikrofon eingeschaltet oder stummgeschaltet ist
- **Core Audio Integration**: Schaltet das Systemmikrofon direkt über die Windows Core Audio API (WASAPI) stumm/aktiv, statt Fensternachrichten zu senden. Absolut sicher in Kombination mit Anti-Cheat-Systemen und voll kompatibel mit modernen Apps wie dem Windows 11 Audiorecorder.
- **Flexibles Klick-Verhalten**: Wähle zwischen Einfachklick oder Doppelklick zum Umschalten des Mikrofons
- **Mehrsprachigkeit**: Deutsch und Englisch mit Live-Sprachwechsel in den Einstellungen
- **Globaler Hotkey**: Programmweit über eine definierbare Tastenkombination
- **Default-State beim Start**: Option zum automatischen Setzen des Mikrofonzustandes beim Programmstart
- **Automatische Zustandsspeicherung**: Der Mikrofonzustand und Einstellungen werden in einer Konfigurationsdatei gespeichert
- **Kontextmenü**: Rechtsklick für zusätzliche Optionen (Mute/Unmute, Einstellungen, Beenden)
- **Sound-Feedback**: Optionale `mic_on.wav` / `mic_off.wav` im Programmverzeichnis aktivieren akustische Rückmeldungen beim Umschalten. Details zur Erstellung und Lizenzierung finden sich unter [Sound-Feedback](#sound-feedback).

<img width="446" height="518" alt="image" src="https://github.com/user-attachments/assets/f731d14e-5d72-4672-9205-720ad44d1f86" />


---

## Voraussetzungen
- Windows (getestet unter Windows 10/11; kompatibel ab Windows Vista)
- .NET Framework 4.0 oder höher
- Zwei Icon-Dateien: `mic_on.ico` und `mic_off.ico` (müssen im gleichen Verzeichnis wie die ausführbare Datei liegen)

---

## Installation
1. **Download:**
   - Lade die [ZIP-Datei](https://github.com/reaperhammer/micmute/releases) herunter
   - Entpacke die ZIP-Datei
   - Kopiere das micmute-Verzeichnis nach **`C:\`**

2. **Icons bereitstellen:**
   - Stelle sicher, dass die Dateien **`mic_on.ico`** und **`mic_off.ico`** im Verzeichnis **`C:\micmute\`** vorhanden sind
   - Du kannst eigene Icons erstellen oder kostenlose Icons von Websites wie IconArchive verwenden

3. **Kompilieren:**
   - Öffne eine Eingabeaufforderung (CMD) und führe den folgenden Befehl aus, um den Code zu kompilieren:
   ```
   C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /out:"C:\micmute\MicMute2.exe" /target:winexe /platform:anycpu /optimize /nologo "C:\micmute\MicMute.cs"
   ```
   - Dies erstellt die ausführbare Datei MicMute2.exe in **`C:\micmute\`**
   - Verschiebe nun den Ordner an einen Ort deiner Wahl

4. **Ausführen:**
   - Starte **`MicMute2.exe`** aus **`C:\micmute\`**
   - Das Tray-Icon erscheint in der Taskleiste und zeigt den Mikrofonzustand an
   - Wird der Zustand nicht korrekt angezeigt, zum Beispiel grünes Icon aber das Mikrofon ist ausgeschaltet:
     - Klicke mit der rechten Maustaste auf das Tray-Icon
     - Wähle "Einstellungen"
     - Setze einen Haken bei "Mikrofon beim Start auf Standardstatus setzen" und wähle den Standardwert aus
   - Das Klick-Verhalten (Einfachklick oder Doppelklick) kann ebenfalls in den Einstellungen konfiguriert werden
   - Die Sprache kann zwischen Deutsch und Englisch gewechselt werden

> [!IMPORTANT]
> **Anti-Cheat-sicher & Als Administrator ausführen**
> MicMute 2 nutzt die native Windows Core Audio API, um das Mikrofon direkt auf Hardware- bzw. Betriebssystemebene stummzuschalten. Dadurch ist die Nutzung absolut sicher und es besteht **kein Risiko** für Banns durch aggressive Anti-Cheat-Systeme (wie Vanguard, Easy Anti-Cheat, BattlEye usw.), da keine Code- oder Fenster-Nachrichten (wie `WM_APPCOMMAND`) in das Spielfenster injiziert werden. Das Tool funktioniert dadurch auch perfekt und zuverlässig mit modernen Windows 11-Apps wie dem Windows-Audiorecorder.
>
> Wenn Sie jedoch ein Spiel spielen, das mit Administratorrechten ausgeführt wird (was bei Spielen mit weitreichenden Anti-Cheat-Systemen wie *Naraka: Bladepoint* üblich ist), **müssen** Sie auch `MicMute2.exe` **als Administrator ausführen**. Dies liegt an der Windows **UIPI** (User Interface Privilege Isolation), die verhindert, dass Standardprogramme globale Hotkeys (`RegisterHotKey`) erfassen können, wenn ein Administrator-Fenster im Vordergrund aktiv ist. Keine Sorge: MicMute verwendet die Administratorrechte ausschließlich zum Registrieren des globalen Hotkeys und greift zu keinem Zeitpunkt auf Ihre Spieldateien oder Prozesse zu.

---

## Konfiguration
### Einstellungen
Rechtsklick auf das Tray-Icon → **Einstellungen** öffnet den Konfigurationsdialog:

- **Globaler Hotkey**: Aktiviere/Deaktiviere und definiere eine Tastenkombination
- **Tray-Icon Klick-Verhalten**: Wähle zwischen Einfachklick oder Doppelklick
- **Sprache**: Wähle zwischen Deutsch und Englisch
- **Standard-Mikrofonstatus**: Lege fest, ob das Mikrofon beim Programmstart automatisch stumm oder aktiv sein soll

### Konfigurationsdatei
Der Mikrofonzustand und alle Einstellungen werden in **`C:\micmute\MicMuteConfig.txt`** gespeichert.

---

## Sound-Feedback

MicMute kann kurze Töne abspielen, wenn das Mikrofon stummgeschaltet oder aktiviert wird. Dies ist besonders nützlich für die Integration mit Tastatur-Firmware (z. B. wenn eine Taste auf dem Ziffernblock für Mute programmiert ist) — so erhalten Sie auch während des Spielens eine akustische Bestätigung.

### Voraussetzungen
- `mic_on.wav` — wird abgespielt, wenn das Mikrofon aktiviert wird
- `mic_off.wav` — wird abgespielt, wenn das Mikrofon stummgeschaltet wird
- Beide Dateien müssen im selben Verzeichnis wie `MicMute2.exe` liegen
- Format: 16-Bit PCM, 22.05 kHz oder 44.1 kHz Samplerate, Mono
- Empfehlung: Halten Sie die Dateien kürzer als 2 Sekunden, um spürbare Verzögerungen zu vermeiden

### Eigene Audiodateien erstellen
Jedes TTS-Tool (Text-to-Speech) kann passende WAV-Dateien erstellen. Die enthaltenen Beispiel-Dateien wurden mit [Mistral Voxtral TTS](https://mistral.ai/news/voxtral) generiert. Beachten Sie, dass die Voxtral-TTS-Ausgabe unter der Lizenz **CC BY-NC 4.0** steht — kostenlos für die Nutzung in Open-Source-Projekten, erfordert jedoch eine kommerzielle Lizenz für kostenpflichtige Produkte.

### Hinweise
- **Icons**: Stelle sicher, dass `mic_on.ico` und `mic_off.ico` im Verzeichnis `C:\micmute\` vorhanden sind, da sie für das Tray-Icon benötigt werden
- **Konfigurationsdatei**: Der Mikrofonzustand wird in **`C:\micmute\MicMuteConfig.txt`** gespeichert
- **Sprachwechsel**: Die Sprache kann jederzeit in den Einstellungen geändert werden und wird sofort angewendet

---

## Lizenz
Dieses Projekt steht unter der GPL-3.0 Lizenz. Du kannst den Code frei verwenden, modifizieren und verteilen, solange die Lizenzbedingungen eingehalten werden.
