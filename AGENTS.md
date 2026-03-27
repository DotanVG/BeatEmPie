# AGENTS.md

This file provides guidance to AI coding agents (Claude Code, Warp, Cursor, Copilot, etc.) when working with code in this repository.

## Project Overview

**BeatEmPie** is a 2D beat-em-up Unity game where a character named **Shushki** fights fish and whales using pies as weapons.

## Unity Setup

- **Engine:** Unity 6 (check `ProjectSettings/ProjectVersion.txt` for exact version)
- **Branch model:** `main` is the stable branch, `dev` is the active development branch
- **Input System:** `Assets/InputSystem_Actions.inputactions` — default Input System action map
- **MCP Integration:** `com.ivanmurzak.unity.mcp` is installed, enabling AI agents to interact directly with the Unity Editor via MCP tools (create GameObjects, modify scenes, run scripts, etc.)

## Current State

Core systems are implemented; combat mechanics are still in progress:

- **Done:** GameManager state machine, player movement/health/events, wave spawning, audio manager with crossfading, dynamic music controller, pie type definitions
- **Placeholder:** Enemy AI (FishEnemy, WhaleEnemy), pie projectile mechanics, StatusEffectHandler, PieInventory↔PlayerCombat wiring, HealthBar/PieHUD
- One scene: `Assets/Scenes/SampleScene.unity`
- Input actions: `Assets/InputSystem_Actions.inputactions`

## Project Structure

```
Assets/
  Art/Sprites/          # Player, Enemies, Pies, UI sprites
  Art/Animations/       # Player animation frames
  Art/Tilemaps/
  Audio/Music/          # 6 procedural soundtrack tracks
  Audio/SFX/
  Prefabs/              # Player, Enemies, FX prefabs
  Scripts/
    Combat/             # PieBase, PieType, pie subclasses (10), StatusEffect
    Enemies/            # EnemyBase, FishEnemy, WhaleEnemy
    Managers/           # GameManager, AudioManager, EnemySpawner, DynamicMusicController, etc.
    Player/             # PlayerController, PlayerStats, PlayerCombat
    UI/                 # HealthBar, PieHUD
  Scenes/               # SampleScene.unity (only scene)
  VFX/
  InputSystem_Actions.inputactions
Packages/               # UPM package manifest
ProjectSettings/        # Unity project settings
.claude/                # Claude Code config and MCP skills
AGENTS.md               # This file
CLAUDE.md               # Architecture guide for Claude Code
```

## Working with This Project

### Opening the Project
Open via Unity Hub → Add → select the `BeatEmPie/` directory. Press Play in the Unity Editor to run the scene.

### MCP Tools (Claude Code / AI agents with Unity MCP support)
If MCP is connected, agents can directly manipulate the Unity Editor:
- Use `scene-list-opened` to inspect the current scene
- Use `gameobject-create`, `gameobject-find`, `gameobject-component-add` etc. to build scenes
- Use `script-update-or-create` to write and compile C# scripts
- Use `console-get-logs` to check for errors after changes
- Use `assets-refresh` after any file system changes

### Coding Conventions
- Language: **C# (.NET, Unity scripting)**
- Follow Unity component-based architecture (MonoBehaviour for gameplay, ScriptableObjects for data)
- Namespace: use `BeatEmPie` as the root namespace
- Keep scripts in `Assets/Scripts/` organized by feature (e.g., `Player/`, `Enemies/`, `Combat/`, `UI/`)

### Game Design Context
- Genre: 2D side-scrolling beat-em-up
- Player character: Shushki
- Enemies: fish and whales
- Combat mechanic: pie-based attacks
- Perspective: 2D (likely using Unity's 2D physics and sprite renderer)

## Git Workflow

- Commit to `dev`, PR to `main`
- Emoji prefix commits matching the existing style (see `git log`)
- Do not commit generated `Library/` or `Temp/` folders (covered by `.gitignore`)
