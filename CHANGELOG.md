# Emotional Garden — Changes Summary

## Scene Management
- **Deleted** `Assets/Main Garden/Scenes/GardenScene.unity` (old unused scene)
- **Renamed** `Assets/Main Garden/Scenes/DefaultGarden.unity` → `Assets/Main Garden/Scenes/GardenScene.unity`
- **Updated** `ProjectSettings/EditorBuildSettings.asset` — build scenes now reference the renamed GardenScene, old entries removed

## New Scripts
- **Assets/Main Garden/Scripts/GardenManager.cs** — Rewrote from scratch (original was corrupted/truncated). Controls sky background switching based on emotion, avatar spawning with scale 2, sorting order assignment, and wiring movement/wander systems
- **Assets/Main Garden/Scripts/GardenMusicManager.cs** — New script. Plays emotion-based looping background music (Calm.mp3, Sad.mp3, Anxious.mp3, Energetic.mp3) using AudioSource. Respects SettingsUI background volume

## Modified Scripts
- **Assets/Scripts/SceneFlow.cs** — Changed `GardenScene` constant from `"DefaultGarden"` to `"GardenScene"`
- **Assets/Minigame/Scripts/MiniGameLauncher.cs** — Changed `ReturnToGarden()` to load `"GardenScene"` instead of `"DefaultGarden"`
- **Assets/Main Garden/Scripts/RuneClickable.cs** — Replaced `SceneManager.LoadScene("MainGarden")` with `MiniGameLauncher.LaunchRandom()`, removed unused `using UnityEngine.SceneManagement`
- **Assets/Main Garden/Scripts/StartSceneUI.cs** — Removed settings button references and handler (settings button removed from start scene)
- **Assets/Main Garden/Scripts/RuneSpawner.cs** — Added sorting order assignment (`sortingOrder = 5`) to spawned runes so they render above plants
- **Assets/Plants/PlantClickHandler.cs** — Replaced cached `Mouse.current`/`Touchscreen.current` with `Pointer.current` for unified mouse+touch support. Added `SettingsUI.isOpen` guard, `EventSystem.IsPointerOverGameObject()` UI check, and `RaycastAll` to penetrate through Boundary collider
- **Assets/Minigame/Scripts/SettingsButton.cs** — Changed `Physics2D.Raycast` to `Physics2D.RaycastAll` so Boundary collider doesn't block clicks. Added `SettingsUI.isOpen` guard
- **Assets/Minigame/Scripts/SettingsUI.cs** — Added "Reselect Emotion" button that navigates to EmotionSelectScene. "Main Garden" button now only shows in minigame scenes (hidden when already in garden). Adjusted panel size and layout for mobile. Updated CanvasScaler reference resolution to 1080x1920 for portrait mobile
- **Assets/Avatar/Scripts/AvatarMovementController.cs** — Replaced `OverlapPoint` boundary check with `bounds.Contains` (works regardless of trigger/physics settings). Added `SettingsUI.isOpen` guard
- **Assets/Minigame/PuzzleGame/DragObject.cs** — Line 116: Replaced deprecated `rb.isKinematic = true` with `rb.bodyType = RigidbodyType2D.Kinematic` (fixes CS0618 warning)

## Scene Objects — GardenScene
- **Added** `GardenController` GameObject with components: AvatarSpawner (all 6 avatar prefabs wired), AvatarMovementController, AvatarRandomWander, GardenManager (sky backgrounds + boundary wired), AudioSource, GardenMusicManager (all 4 music clips wired)
- **Added** `SadBackground` GameObject — SpriteRenderer with `Sad_Sky/1.png`, sortingOrder -10, child `SadCloud` with black cloud sprite at sortingOrder -5. Starts inactive
- **Added** `AnxiousBackground` GameObject — SpriteRenderer with `Anxious_Sky/1.png`, sortingOrder -10, child `AnxCloud` with gray cloud sprite at sortingOrder -5. Starts inactive
- **Added** `EnergeticBackground` GameObject — SpriteRenderer with `Energetic_Sky/1.png`, sortingOrder -10, child `Sun` with Sun.png sprite at sortingOrder -5. Starts inactive
- **Added** `SettingsBtn` GameObject — SpriteRenderer with `settings_icon.png` (sortingOrder 100), BoxCollider2D, SettingsButton script
- **Modified** `CalmBackground` — sortingOrder set to -10
- **Modified** `Plant1–Plant9` — sortingOrder set to 2 (above tilemap at 0)
- **Modified** `Boundary` — BoxCollider2D.isTrigger set to true (prevents blocking raycasts to plants/runes/settings)
- **Removed** `Male_Black` (leftover avatar instance from scene, now spawned dynamically)

## Scene Objects — StartScene
- **Removed** `SettingsButton` GameObject from StartScene hierarchy

## Prefab Changes
- **All 6 avatar prefabs** (`Male_Light`, `Male_Black`, `Male_Tan`, `Female_Light`, `Female_Black`, `Female_Tan`) — localScale set to (2, 2, 1)

## Sorting Order Hierarchy
```
-10  Sky backgrounds (Calm, Sad, Anxious, Energetic)
 -5  Cloud/sun/lightning children of backgrounds
  0  Tilemap (ground tiles)
  2  Plants
  5  Runes (set at runtime by RuneSpawner)
 10  Avatar (set at runtime by GardenManager)
100  Settings button
```

## Deleted Files (Cleanup)
- `Assets/Creature.zip` — Raw zip archive, not a Unity asset
- `Assets/Editor/Coplay/*.cs` — 19 one-time setup/preview/fix helper scripts
- `Assets/Editor/AddMainGardenToBuild.cs`
- `Assets/Editor/CheckAllAudioSources.cs`
- `Assets/Editor/CheckAudioSources.cs`
- `Assets/Editor/CheckBuildScenes.cs`
- `Assets/Editor/CheckPlantSprites.cs`
- `Assets/Editor/CleanBuildScenes.cs`
- `Assets/Editor/FixPlantSprites.cs`
- `Assets/Editor/SaveCurrentScene.cs`
- `Assets/Editor/SetupBuildScenes.cs`
- `Assets/Editor/SetupSettingsButtons.cs`
- `Assets/Editor/UpdateAllSettingsButtons.cs`
