# 🥧 BeatEmPie

A 2D side-scrolling beat em up built in Unity 6 (Built-In Render Pipeline).

## Concept
Shushki fights waves of fish and whales by dropping magical pies from the sky.
Each pie type has a unique power — master them all to survive the onslaught.

## Team
- Dotan — Developer (programming, design, project management)
- Romi — Artist (character art, animations, environment)
- Noam — Composer (SFX, soundtrack)

## Pie Arsenal 🥧
| Pie | Effect |
|-----|--------|
| 🥧 Apple Pie | Standard drop — basic damage, always available |
| 🍒 Cherry Pie | Explosive AOE — knocks back nearby enemies |
| 🫐 Blueberry Pie | Freeze — slows/freezes enemy for a duration |
| 🍋 Lemon Meringue | Chain Lightning — zaps between nearby enemies |
| 🍓 Strawberry Pie | Homing — locks onto nearest enemy, cannot miss |
| 🥩 Meat Pie | Heavy — massive damage, slow charge, cracks ground |
| 🍄 Mushroom Pie | Confusion — enemy turns and attacks their own allies |
| 🎃 Pumpkin Pie | Ultimate AOE — screen-wide blast, very limited uses |
| 🍫 Chocolate Pie | DOT — leaves puddle that slows and damages over time |
| 🌶️ Chili Pie | Fire Trail — leaves burning zone enemies walk through |

## Tech Stack
- Unity 6, 2D Built-In Render Pipeline
- Target platforms: PC, Mac, WebGL
- Resolution: 1920x1080

## Folder Structure

```
Assets/
├── Art/Sprites/ (Player, Enemies, Pies, UI)
├── Art/Animations/
├── Art/Tilemaps/
├── Audio/SFX/
├── Audio/Music/
├── Scripts/Player/
├── Scripts/Enemies/
├── Scripts/Combat/ (Pies/, Effects/)
├── Scripts/Managers/
├── Scripts/UI/
├── Prefabs/ (Player, Enemies, FX)
├── VFX/
├── Scenes/
└── Settings/
```
