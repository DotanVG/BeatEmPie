# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

**BeatEmPie** — 2D side-scrolling beat-em-up in Unity 6 (Built-In Render Pipeline). Shushki fights waves of fish and whales by dropping magical pies from the sky.

- **Engine:** Unity 6, 2D Built-In Render Pipeline
- **Platforms:** PC, Mac, WebGL — Resolution: 1920×1080
- **Branch model:** `dev` is active development, `main` is stable

## Unity Development

Open via Unity Hub → Add → select the `BeatEmPie/` directory. Press Play to run. There is only one scene: `Assets/Scenes/SampleScene.unity`.

MCP is connected (`com.ivanmurzak.unity.mcp`), so you can interact with the Unity Editor directly:
- `scene-list-opened` / `scene-get-data` — inspect the scene
- `gameobject-find`, `gameobject-create`, `gameobject-component-add` — build scenes
- `script-update-or-create` — write and compile C# scripts
- `console-get-logs` — check for errors after changes
- `assets-refresh` — sync after file system changes

## Git Workflow

- Commit to `dev`, PR to `main`
- Emoji-prefix commits matching existing style (see `git log`)

## Code Conventions

- **Namespace:** `BeatEmPie` (root for all scripts)
- **Scripts location:** `Assets/Scripts/`, organized by feature subfolder
- MonoBehaviour for gameplay components, ScriptableObjects for data assets
- Event-driven decoupling via C# `event Action` — subscribe, don't poll
- Singleton managers use `DontDestroyOnLoad` with a static `Instance`

## Architecture

### Systems Overview

**GameManager** (`Scripts/Managers/GameManager.cs`) — Central state machine (MainMenu → Playing → Paused/GameOver/Victory). Fires `OnStateChanged` and `OnScoreChanged` events. All other systems respond to these events rather than checking state directly.

**Player** (three components on Shushki prefab):
- `PlayerStats` — health, `OnHealthChanged` / `OnDeath` events
- `PlayerController` — physics movement + sprite animation (Idle/Walk/Jump)
- `PlayerCombat` — throw cooldown, input mapping; **pie throwing not yet wired to PieInventory**

**EnemySpawner** (`Scripts/Managers/EnemySpawner.cs`) — Wave manager singleton. Escalates fish count per wave (`4 + wave - 1`); spawns a whale boss every N waves (default: every 3). Fires `OnWaveStarted`, `OnWaveCleared`, `OnWhaleSpawned`. Enemies call `NotifyEnemyDied()` on death.

**Combat / Pie System:**
- `PieType` enum — 10 pie types (Apple, Cherry, Blueberry, LemonMeringue, Strawberry, Meat, Mushroom, Pumpkin, Chocolate, Chili)
- `PieBase` — abstract projectile; override `OnImpact()` in each subclass
- Individual pie scripts in `Scripts/Combat/Pies/` — all outlined, most not yet implemented
- `PieInventory` — tracks unlocked pies, cooldown per type, cycling; **not yet connected to PlayerCombat**
- `StatusEffectHandler` — tracks timed effects (Frozen, Burning, Confused, etc.) on enemies; outlined but not implemented

**Enemy AI** (`Scripts/Enemies/`):
- `EnemyBase` — abstract base with intended state machine (Idle/Chase/Attack/Stunned/Dead)
- `FishEnemy`, `WhaleEnemy` — outlined but AI logic not yet implemented

**Audio:**
- `AudioManager` — singleton with two AudioSources for crossfading music + one for SFX
- `DynamicMusicController` — adapts music based on enemy count (Calm → Intense at ≥5 enemies → BossFight on whale spawn); polls every 2s
- `MusicTrack` enum: None, MainMenu, GameplayCalm, GameplayIntense, BossFight, Victory, GameOver
- `SFXLibrary` — ScriptableObject centralizing all audio clips; `GetPieImpact(PieType)` with fallback

**UI:**
- `HealthBar` — slider driven by PlayerStats events
- `PieHUD` — current pie selection + cooldown overlays

### What's Working vs. Placeholder

| System | Status |
|--------|--------|
| GameManager state machine | Done |
| Player movement + physics | Done |
| Player health/death events | Done |
| Wave spawning + escalation | Done |
| Audio manager + crossfade | Done |
| Dynamic music controller | Done |
| Pie type definitions | Done |
| Enemy AI (FishEnemy, WhaleEnemy) | Placeholder |
| Pie projectile mechanics | Placeholder |
| StatusEffectHandler | Placeholder |
| PieInventory ↔ PlayerCombat wiring | Not started |
| HealthBar / PieHUD | Placeholder |
