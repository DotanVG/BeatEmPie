# AGENTS.md

This file provides guidance to AI coding agents (Claude Code, Warp, Cursor, Copilot, etc.) when working with code in this repository.

## Project Overview

**BeatEmPie** is a 2D beat-em-up Unity game where a character named **Shushki** fights fish and whales using pies as weapons. The project is in early development — no gameplay scripts exist yet.

## Unity Setup

- **Engine:** Unity 6 (check `ProjectSettings/ProjectVersion.txt` for exact version)
- **Branch model:** `main` is the stable branch, `dev` is the active development branch
- **Input System:** `Assets/InputSystem_Actions.inputactions` — default Input System action map
- **MCP Integration:** `com.ivanmurzak.unity.mcp` is installed, enabling AI agents to interact directly with the Unity Editor via MCP tools (create GameObjects, modify scenes, run scripts, etc.)

## Current State

- Only a default `SampleScene` and the auto-generated Input System actions exist
- No C# scripts, prefabs, sprites, or audio assets yet
- Packages: Unity 2D Feature pack, Input System, AI Assistant, AI Inference, Test Framework

## Project Structure

```
Assets/
  Scenes/         # Unity scenes
  InputSystem_Actions.inputactions
Packages/         # UPM package manifest
ProjectSettings/  # Unity project settings
.claude/          # Claude Code config and MCP skills
AGENTS.md         # This file
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
