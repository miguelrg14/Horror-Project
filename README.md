# Horror Project

[![CI](https://github.com/miguelrg14/Horror-Project/actions/workflows/ci.yml/badge.svg)](https://github.com/miguelrg14/Horror-Project/actions/workflows/ci.yml)
[![Play in the browser](https://img.shields.io/badge/play-in%20the%20browser-8b0000)](https://miguelrg14.github.io/Horror-Project/)
[![Release](https://img.shields.io/github/v/release/miguelrg14/Horror-Project?label=download)](https://github.com/miguelrg14/Horror-Project/releases/latest)
![Unity 2020.3](https://img.shields.io/badge/Unity-2020.3%20LTS-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-8.0-239120?logo=csharp)

First-person horror prototype made in Unity by a team of three students during the second year
of the Video Game Design & Development degree at ESNE. It was shown at **CometCon Gijón 2022**.

A door blocks your way out of a dark house. Explore it with a lantern that needs batteries, find the key
hidden in each level, inspect objects in 3D, keep them in your inventory and open the three locks on the
final door.

**▶ [Play it in the browser](https://miguelrg14.github.io/Horror-Project/)** · or download the Windows / Linux build from the
[latest release](https://github.com/miguelrg14/Horror-Project/releases/latest).
The original team page is on [itch.io](https://lucasgarcia002.itch.io/horror-project).

![Main menu](docs/screenshots/01-main-menu.png)

| Level 1 | Pause menu |
|---|---|
| ![Level 1](docs/screenshots/03-level1.png) | ![Pause menu](docs/screenshots/04-pause-menu.png) |
| **Options** | **Level 3** |
| ![Options](docs/screenshots/02-options.png) | ![Level 3](docs/screenshots/06-level3.png) |

*Screenshots are taken automatically by the CI smoke test from a real Linux build.*

## Contents

| Scene | What happens |
|---|---|
| `Menu` | Main menu with looping theme, options panel and credits |
| `Level1` – `Level3` | Explore the house, find the level's key and open one of the three locks; each lock opens the door to the next level |
| `Credits` | Scrolling credits that return to the menu |

## My contributions

This was a team project. My part (Miguel Rodríguez Gallego) was the **audio, the menus and options,
the music and sound design, and part of the 3D art**:

| Area | What I did |
|---|---|
| Audio system | Audio channels (player SFX, music, interface, voices, world sounds) routed through an `AudioMixer`, so every volume can be set from the options. Gameplay code only calls `PlayerSFXManager.instance.PlaySFX("pickKey")`. |
| Menus | Main menu, in-game pause menu (time stops, heartbeat loop, cursor release), options panel shared by both, credits screen |
| Options | Volume per channel, resolution, quality, fullscreen, brightness and mouse sensitivity, saved between sessions |
| Footsteps | Footstep loop in the player controller with random volume and pitch per step |
| Music & SFX | All the music, composed in FL Studio 20 (e.g. the *Terror Pad* ambience, built with granular synthesis), menu hits, and sounds I recorded and cleaned up in Audacity (backpack, battery and note pick-ups...) |
| 3D | Props modelled and textured in Substance, e.g. the armchair, modelled with subdivision surfaces and reduced to a low-poly mesh |

Teammates: **Lucas García Domínguez** programmed the gameplay (movement and camera, interaction,
inventory, item viewer, lantern, doors, chains, safe mini-game and level flow) and lighting;
**Samuel Robinson Fuentes** built the modular house and the first placeholder models.
Their code is kept as they wrote it.

### What changed in this portfolio version

The game was cleaned up years after the course; the gameplay is the same.

- **Audio refactor** – the six copy-pasted managers became one generic `AudioChannel<T>` base class
  with a name → clip `SoundBank`. One-shot sounds clean themselves up and looping sounds (menu theme,
  pause heartbeat) are no longer cut after one play.
- **Options fixes** – volume sliders now go from 0 to 1 and are converted to decibels (the original
  slider mapped straight to −80…0 dB and was nearly silent for half its range); settings are applied on
  start; the sensitivity slider now affects the camera; the resolution list is filled before a value is
  selected; Escape inside the options goes back to the pause menu.
- **Bugs** – footsteps did not play on some diagonals (`x + y != 0` was the "moving" check); the
  player build failed because a gameplay script imported `UnityEditor`.
- **Clean-up** – removed a paid third-party lightmapper, unused scenes, dead scripts and the source art
  files; textures downscaled to their import size and long WAV files converted to Ogg. The project went
  from about 4 GB to ~390 MB and no longer needs Git LFS.
- **Tests and CI** – unit tests, play-mode tests, builds for three platforms, a headless smoke test and
  automatic deployment of the WebGL build.

## Controls

| Action | Input |
|---|---|
| Move | `W` `A` `S` `D` |
| Look | Mouse |
| Interact / pick up | Left click |
| Rotate an item in the inspector | Drag with left click |
| Keep the inspected item / close the inspector | `X` / `E` |
| Inventory | `Q` |
| Lantern on / off | Right click |
| Zoom | `Space` |
| Pause menu | `Esc` |

## Running it

- **Browser:** <https://miguelrg14.github.io/Horror-Project/> (desktop browser; click the game to capture the mouse).
- **Windows / Linux:** download the zip from the [latest release](https://github.com/miguelrg14/Horror-Project/releases/latest), unzip and run `HorrorProject.exe` or `./HorrorProject`.
- **Editor:** open `UnityProject/` with **Unity 2020.3.27f1** and play the `Menu` scene
  (`Assets/_Project/Scenes`). Every asset is in plain Git, so a normal clone or *Download ZIP* is enough.

## Project structure

```
UnityProject/
  Assets/
    _Project/
      Scripts/
        Core/         engine-independent logic (sound bank, settings, pause states, volume maths...)
        Audio/        audio channels                                   ← mine
        Settings/     options panel, volume, sensitivity               ← mine
        UI/           pause menu, credits, level loading
        Player/ Interaction/ Inventory/ Lantern/ Doors/ MiniGame/ ...  gameplay (teammates)
        Diagnostics/  smoke-test runner used by CI
      Tests/
        EditMode/     unit tests of Core
        PlayMode/     tests that load the real scenes
      Scenes/ Prefabs/ Art/ Audio/ Animations/ Fonts/ Resources/
    ThirdParty/       GDT fade effect, two Unity Standard Assets shaders
tests/Core.Tests/     runs the EditMode tests with plain .NET (no Unity licence needed)
.github/workflows/    CI and release pipelines
```

## Design notes

- **Audio channels.** Each channel (`PlayerSFXManager`, `PlayerMusicManager`, `PlayerUIManager`,
  `PlayerVoiceManager`, `WorldSFXManager`, `WorldVoiceManager`) derives from `AudioChannel<T>`. A sound
  is played on a short-lived object created from a template whose `AudioSource` is routed to the
  channel's mixer group, so sounds can overlap and each volume slider controls exactly one group.
  The static `instance` keeps the call site short for the rest of the team.
- **Logic without the engine.** Sound lookup, volume conversion, settings storage, the pause state
  machine, the credits countdown and the footstep rules live in `HorrorProject.Core` (namespace `Horror.Core`), an assembly with no
  `UnityEngine` reference. The MonoBehaviours only wire them to the scene, which is what makes them
  testable with plain .NET in a few seconds.
- **Settings.** `GameSettings` wraps an `ISettingsStorage` (PlayerPrefs in the game, a dictionary in the
  tests), clamps every value and migrates the old decibel volumes. The sensitivity is applied by a small
  component that scales the mouse input between the input script and the camera script (via the script
  execution order), so the teammates' camera code did not have to change.
- **Mobile port study.** As a separate assignment I analysed how the game would be adapted to phones:
  baked lighting with at most one real-time light (the lantern), hard or no shadows, lower-resolution
  textures, larger touch-friendly menus with fewer options, on-screen sticks in the lower corners with the
  lantern and pick-up buttons above the right one, touch rotation in the 3D item viewer, and a premium
  model (a 2.99 € game of five levels with a free first level as a demo, followed by episodic sequels).
  The team's feedback was to keep more of the lighting for atmosphere and to let players rearrange the
  touch controls.

## Tests

| Suite | What it covers | Where it runs |
|---|---|---|
| EditMode (NUnit) | `SoundBank`, `VolumeMath`, `GameSettings`, `PauseStateMachine`, `CountdownTimer`, `ResolutionOptions`, `FootstepRules` | Unity Test Runner and `dotnet test tests/Core.Tests` |
| PlayMode | Loads the real scenes: menu theme starts and loops, pause/resume with Escape, one-shot sounds clean up, credits return to the menu | Unity Test Runner |
| Smoke test | A real Linux build is started under a virtual display with `-smoke-test`: menu, options, all three levels, pause menu, credits; screenshots are saved and any logged error fails the run | CI |

```bash
dotnet test tests/Core.Tests          # core logic, no Unity needed
```

The CI workflow (`.github/workflows/ci.yml`) runs the .NET tests, the Unity tests, builds Linux,
Windows and WebGL players, runs the smoke test and publishes the WebGL build to GitHub Pages from `main`.
Pushing a `v*` tag runs `release.yml`, which attaches the Windows, Linux and WebGL builds to a GitHub release.

## Credits

- **Miguel Rodríguez Gallego** – audio programming, menus and options, music, sound design, 3D props
- **Lucas García Domínguez** – gameplay programming, lighting, 3D models
- **Samuel Robinson Fuentes** – level construction, early 3D models
- Fonts: [Special Elite](https://fonts.google.com/specimen/Special+Elite) and [Cinzel](https://fonts.google.com/specimen/Cinzel) (SIL Open Font License)
- Scene fade: GDT Fade Effect script (third party); glass and projector shaders from Unity Standard Assets
- Some PBR textures and sound effects come from free online libraries

Code released under the [MIT License](LICENSE).
