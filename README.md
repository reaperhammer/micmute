# MicMute 2

**Version:** v2.1.2  
**Last Updated:** 2026-05-24  
**Authors:**  
- AveYo (Original, no longer available on GitHub)  
- reaperhammer (Improvements, Core Audio integration, and audio feedback)

MicMute is a small Windows utility that allows you to quickly mute or unmute your microphone via a system tray icon. It displays the current microphone status (on/off) and stores the state in a configuration file.

![mic-on-off](https://github.com/user-attachments/assets/5277a8af-3598-4b3c-a46c-df598fce5b6c)

---

## Features
- **System tray icon**: Indicates whether the microphone is enabled or muted
- **Core Audio Integration**: Toggles the system microphone state directly via Windows Core Audio API (WASAPI) instead of injecting windows messages. 100% safe to use alongside aggressive anti-cheat systems. Works reliably with modern applications like the Windows 11 Sound Recorder.
- **Flexible click behavior**: Choose between single-click or double-click to toggle the microphone
- **Multi-language support**: German and English with live language switching in settings
- **Global hotkey**: Application-wide control using a configurable key combination
- **Default startup state**: Option to automatically set the microphone state when the program starts
- **Automatic state persistence**: Microphone status and settings are stored in a configuration file
- **Context menu**: Right-click for additional options (Mute/Unmute, Settings, Exit)
- **Sound files**: Optional `mic_on.wav` / `mic_off.wav` in the exe directory enable audio feedback. See [Sound Feedback](#sound-feedback) for generation notes and licensing.

<img width="446" height="518" alt="image" src="https://github.com/user-attachments/assets/3c3ac07e-8555-4a56-a594-c3b4286fee0e" />

---

## Requirements
- Windows (tested on Windows 10/11; compatible from Windows Vista)
- .NET Framework 4.0 or higher
- Two icon files: `mic_on.ico` and `mic_off.ico` (must be located in the same directory as the executable)
- Optional: `mic_on.wav` and `mic_off.wav` for audio feedback (see [Sound Feedback](#sound-feedback))

---

## Installation
1. **Download**
   - Download the [ZIP file](https://github.com/reaperhammer/micmute/releases)
   - Extract the archive
   - Copy the `micmute` folder to **`C:\`**

2. **Provide icons**
   - Ensure the files **`mic_on.ico`** and **`mic_off.ico`** are located in **`C:\micmute\`**
   - You can create your own icons or download free ones from sites such as IconArchive

3. **Compile**
   - Double-click `buildme.bat` in the repo root to compile, OR manually:
   ```
   C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe /out:"C:\micmute\MicMute2.exe" /target:winexe /platform:anycpu /optimize /nologo "C:\micmute\MicMute.cs"
   ```
   - This creates the executable **MicMute2.exe** in **`C:\micmute\`**
   - You may then move the folder to a location of your choice

4. **Run**
   - Start **`MicMute2.exe`** from **`C:\micmute\`**
   - The tray icon will appear in the taskbar and display the microphone state
   - If the status is incorrect (e.g., green icon but microphone is muted):
     - Right-click the tray icon
     - Select **Settings**
     - Enable **Set microphone to default state on startup** and choose the desired default state
   - Click behavior (single-click or double-click) can also be configured in settings
   - Language can be switched between English and German

> [!IMPORTANT]
> **Anti-Cheat Safe & Running as Administrator**
> MicMute 2 uses the native Windows Core Audio API to mute/unmute your microphone at the hardware/OS level. This means it is 100% safe to use alongside aggressive anti-cheat software (like Vanguard, Easy Anti-Cheat, BattlEye, etc.) and will not trigger any bans since it never injects code or window messages (like `WM_APPCOMMAND`) into game windows. It also ensures 100% compatibility with modern applications like the Windows 11 Sound Recorder.
>
> However, if you are playing a game that runs as Administrator (which is common for games with high-privilege anti-cheats, e.g. *Naraka: Bladepoint*), you **must** run `MicMute2.exe` as **Administrator** as well. This is due to Windows **UIPI** (User Interface Privilege Isolation), which prevents standard applications from detecting global hotkeys (`RegisterHotKey`) while a high-privilege window is active. Rest assured, MicMute only uses Administrator rights to register the global hotkey and does not touch or interact with your game files/processes in any way.


---

## Configuration
### Settings
Right-click the tray icon → **Settings** opens the configuration dialog:

- **Global Hotkey**: Enable/disable and define a key combination
- **Tray Icon Click Behavior**: Choose between single-click or double-click
- **Language**: Choose between English and German
- **Default Microphone State**: Set whether the microphone should automatically be muted or active on program startup

### Configuration File
The microphone state and all settings are stored in **`C:\micmute\MicMuteConfig.txt`**.

---

## Sound Feedback

MicMute can play short audio cues when the microphone is muted or unmuted. This is useful for keyboard firmware integration (e.g., programming a numpad key to trigger mic mute) — you get audible confirmation even while gaming.

### Requirements
- `mic_on.wav` — plays when microphone is unmuted
- `mic_off.wav` — plays when microphone is muted
- Both files must be in the same directory as `MicMute2.exe`
- Format: 16-bit PCM, 22.05 kHz or 44.1 kHz sample rate, mono
-建议: keep files under 2 seconds to avoid noticeable delay

### Generating your own audio files
Any TTS tool can generate suitable WAV files. The included samples were generated with [Mistral Voxtral TTS](https://mistral.ai/news/voxtral). Note that Voxtral TTS output is licensed under **CC BY-NC 4.0** — free for open-source use, but requires a commercial license for paid products.

### Notes
- **Icons**: Ensure `mic_on.ico` and `mic_off.ico` are present in `C:\micmute\`, as they are required for the tray icon
- **Configuration file**: The microphone state is stored in **`C:\micmute\MicMuteConfig.txt`**
- **Language switching**: The language can be changed at any time in settings and is applied immediately

---

## License
This project is licensed under the GPL-3.0 license. You are free to use, modify, and distribute the code as long as the license terms are respected.
