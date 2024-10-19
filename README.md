# MISI emu autosplitter thingy
An automatic timer pauser for **Monsters, Inc. Scare Island** speedrunning **(PCSX2 only)**
### ✨ Current Features
- Pauses your LiveSplit timer when watching a pre-rendered cutscene or loading the next map
- A console that logs the game state and errors with timestamps
### 🚧 Planned
- Autosplitting when getting medals
- Pausing game time instead of real time (and accessing LiveSplit internally instead of pressing a key)
##
### ⚠️ Disclaimer
- This is not a scriptable auto splitter (ASL) for LiveSplit, but rather an external program. It's aimed to be a lowkey and power efficient background program
- Since this program relies on Memory.dll, your antivirus will very likely identify it as a false positive
### 📋 Notes
- Any version of Scare Island works, as long as it's the PAL-only PS2 release
- Every PCSX2 version is supported (latest nightly is recommended)
- Your LiveSplit's pause shortcut has to be P
- Enable "Global Hotkeys" in LiveSplit's settings (recommended)

## Tutorial
### Initial Setup
1. Download the [latest release](https://github.com/fr4nk014/MISI-emu-autosplitter-thingy/releases)
2. Extract the ZIP to a folder
3. Run **MISI-emu-autosplitter-thingy.exe**
### Starting a Run
1. Run PCSX2 and boot Scare Island
2. Create your run savestate (New Game -> Select Mike -> Press F1 when Mike comes out of the entrance elevator)
3. Open your splits and start running
- Enable "Global Hotkeys" from your LiveSplit settings
- Remember that this program manually presses your P key, so having the emulator window focused is recommended.
