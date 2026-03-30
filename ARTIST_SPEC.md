# BeatEmPie — Artist Specification

> Game: 2D side-scrolling beat-em-up. Resolution: 1920×1080. Engine: Unity 6 Built-In RP.
> Style direction: **Colorful, cartoon/comic-book, chunky outlines, slightly silly**.
> Think: Cuphead meets Kirby — expressive characters, exaggerated squash/stretch.
> All sprites are **PNG with transparency**. Spritesheets in one row, uniform frame width.

---

## 1. PLAYER CHARACTER — Shushki

Shushki is a small round magical girl who throws pies. Chubby, big eyes, cute hat.

### Spritesheet: `Assets/Art/Sprites/Player/Shushki_Sheet.png`

| State        | Frames | FPS  | Notes                                              |
|--------------|--------|------|----------------------------------------------------|
| Idle         | 4      | 6    | Gentle breathing bob, hat wobble                   |
| Walk         | 6      | 12   | Waddling run, arms swinging                        |
| Jump (rise)  | 2      | 8    | Tuck into ball, hair pops up                       |
| Jump (fall)  | 2      | 8    | Arms flail, hair flows down                        |
| Land         | 2      | 10   | Squash on landing, pop back                        |
| Throw        | 5      | 16   | Wind-up, release, follow-through. Pie leaves hand at frame 3 |
| Hurt         | 3      | 12   | Flash red, recoil backward                         |
| Death        | 6      | 10   | Spiral fall, stars circling, fade out              |

**Frame size:** 64×64 px per frame. Scale to ~1.2 Unity units in game.
**Facing direction:** Sprite faces RIGHT (code flips X for left movement).
**Pivot:** Bottom-center of sprite.

### Portrait / Icon
- `Shushki_Portrait.png` — 128×128 face close-up for UI (health bar label, pause screen)

---

## 2. ENEMIES

### 2a. Fish Enemy (Basic Grunt)

A googly-eyed fish standing upright on tiny legs. Carries a tiny fork as a weapon.
**Placeholder color:** Blue rectangle.

**Spritesheet:** `Assets/Art/Sprites/Enemies/Fish_Sheet.png`

| State     | Frames | FPS  | Notes                                          |
|-----------|--------|------|------------------------------------------------|
| Idle      | 4      | 6    | Gill flaps, eye blink every ~2s                |
| Swim/Walk | 6      | 12   | Fins flap, legs waddle comically               |
| Attack    | 4      | 14   | Lunges forward with fork, recoils              |
| Hit       | 2      | 14   | Flash white/red, recoil                        |
| Death     | 5      | 10   | Eyes go X, flips onto back, fades              |

**Frame size:** 48×48 px. ~0.8 Unity units wide.
**Status effect tints** (handled by code, art just needs base sprite):
- Frozen: blue tint overlay ❄️
- Burning: orange-red tint overlay 🔥
- Confused: purple spiral eyes overlay
- Stunned: yellow stars overhead

### 2b. Whale Enemy (Boss)

Enormous sperm whale wearing a tiny top hat and monocle. Very dramatic.
**Placeholder color:** Purple rectangle (2× scale).

**Spritesheet:** `Assets/Art/Sprites/Enemies/Whale_Sheet.png`

| State        | Frames | FPS  | Notes                                              |
|--------------|--------|------|----------------------------------------------------|
| Idle         | 4      | 5    | Gentle float, monocle gleam, hat tilt              |
| Walk/Float   | 6      | 10   | Slow majestically forward, fins paddling           |
| Charge Windup| 4      | 8    | Coils back, steam from blowhole, dramatic pause    |
| Charge       | 3      | 16   | LUNGES forward, motion blur lines                  |
| Slam         | 4      | 12   | Slams down, ground crack, shockwave rings          |
| Hit          | 2      | 14   | Flash white, wobble                                |
| Death        | 8      | 10   | Dramatic twirl, tiny top hat floats up, big splash |

**Frame size:** 128×80 px. ~2.4 Unity units wide. Boss is BIG — takes up ~1/4 of screen height.

### 2c. Future Characters (placeholder slots)
Reserve spritesheet slots for: Crab, Shark, Octopus (future waves).

---

## 3. PIE PROJECTILES

All pies are **circular** — spinning during flight, impact burst on hit.
Base size: 48×48 px for the pie, 80×80 px for the impact burst.

**Spritesheet per pie:** `Assets/Art/Sprites/Pies/[PieName]_Sheet.png`

Each pie needs **3 animation states**:

| State       | Frames | FPS | Description                                  |
|-------------|--------|-----|----------------------------------------------|
| Fly (loop)  | 4      | 12  | Full spin, slight deformation from speed     |
| Impact      | 5      | 16  | Explode/splat burst, then fade               |
| Icon        | 1      | —   | Static 64×64 icon for hotbar slot            |

### Pie Designs:

| #  | Type           | Color         | Design Description                                         |
|----|----------------|---------------|------------------------------------------------------------|
| 1  | Apple          | Yellow-green  | Classic lattice-top pie, leaf detail on crust              |
| 2  | Cherry         | Deep red      | Cherries visible through lattice, oozes red filling        |
| 3  | Blueberry      | Blue-purple   | Blueberries in filling, frost crystals on crust edge       |
| 4  | Lemon Meringue | Bright yellow | Fluffy white meringue peak, sparks/lightning crackling off |
| 5  | Strawberry     | Pink          | Strawberry slices visible, heart-shaped steam rising       |
| 6  | Meat           | Dark brown    | Thick pastry, meat juices, steam, heavy/dense look         |
| 7  | Mushroom       | Purple/grey   | Mushroom cap motifs on crust, purple spores floating       |
| 8  | Pumpkin        | Orange        | Carved jack-o-lantern face on the crust, glowing           |
| 9  | Chocolate      | Dark brown    | Glossy chocolate ganache dripping off sides                |
| 10 | Chili          | Orange-red    | Chili peppers on top, heat waves, tiny flames on edge      |

### Special Impact FX (per pie):
- Apple: Simple splat star
- Cherry: **Explosion ring** + debris chunks flying out
- Blueberry: **Ice shatter** — crystalline burst, freezing ripple
- Lemon Meringue: **Lightning arc** — 3 bolt branches
- Strawberry: **Heart burst** — hearts pop out on impact
- Meat: **Ground crack** — crevice lines + dust cloud
- Mushroom: **Spore cloud** — purple puff with swirls, question marks
- Pumpkin: **SCREEN FLASH** — massive orange explosion ring, 2× size
- Chocolate: **Chocolate puddle** — spreading brown goo circle (persists as `FireZone` visual)
- Chili: **Fire explosion** + **fire trail decal** that lingers (the `FireZone` sprite)

---

## 4. PIE DROPS (Collectibles)

**Spritesheet:** `Assets/Art/Sprites/Pies/PieDrop_Sheet.png`

Floating version of each pie icon, slightly smaller, with a sparkle/glow aura.

| State      | Frames | FPS | Description                            |
|------------|--------|-----|----------------------------------------|
| Float      | 4      | 8   | Gentle bob up/down, slow rotation      |
| Collected  | 3      | 16  | Pop + sparkle burst, shrinks to zero   |

One sheet with all 10 pie drop variants **stacked vertically** (10 rows × 4 frames).
Frame size: 40×40 px each.

---

## 5. STATUS EFFECT OVERLAYS

Visual overlays drawn over an enemy when they have a status effect.
All are **looping particle-like animations**.
File: `Assets/Art/FX/StatusEffects/[EffectName]_Overlay.png`

| Effect      | Visual Description                                          | Frames | FPS |
|-------------|-------------------------------------------------------------|--------|-----|
| Frozen      | Ice crystal overlay + frost breath puffs                    | 4      | 6   |
| Burning     | Orange/red flame particles rising from enemy               | 6      | 14  |
| Confused    | Purple spiral circles around enemy's head + question marks  | 6      | 10  |
| Slowed      | Brown drips/chocolate goo ooze effect overlay               | 4      | 8   |
| Stunned     | Yellow stars orbiting enemy's head                          | 4      | 10  |
| Electrified | White/yellow lightning arcs crackling across enemy body     | 4      | 16  |

---

## 6. ENVIRONMENT / BACKGROUND

**File:** `Assets/Art/Sprites/Background/`

### Background Layers (parallax, left-to-right scroll)
| Layer | File              | Description                                                    |
|-------|-------------------|----------------------------------------------------------------|
| Sky   | BG_Sky.png        | Gradient sky — dawn/dusk palette. Pink-orange-purple.          |
| Far   | BG_Far.png        | Distant mountains or cityscape silhouette                       |
| Mid   | BG_Mid.png        | Rolling hills with giant pie factories/bakeries in the distance |
| Near  | BG_Near.png       | Foreground bushes / grassy patches                              |

**Dimensions:** 3840×1080 px (2× screen width for scrolling). Each layer PNG.

### Ground Platform
| File                    | Description                                              |
|-------------------------|----------------------------------------------------------|
| Ground_Tile.png         | 128×128 tiling ground texture — cobblestones with grass  |
| Ground_Edge_Left.png    | Left edge of platform                                    |
| Ground_Edge_Right.png   | Right edge of platform                                   |

---

## 7. UI ELEMENTS

### 7a. Pie Hotbar
**File:** `Assets/Art/UI/Hotbar/`

| File                  | Size      | Description                                         |
|-----------------------|-----------|-----------------------------------------------------|
| Hotbar_BG.png         | 660×80    | Stone/rustic bar background for 10 slots            |
| Slot_Normal.png       | 58×58     | Empty slot border (normal state)                    |
| Slot_Selected.png     | 62×62     | Highlighted slot border (yellow glow, slightly bigger) |
| Slot_Locked.png       | 58×58     | Greyed-out slot with padlock overlay                |
| Slot_Empty.png        | 58×58     | Unlocked but 0 quantity — faded                    |
| Cooldown_Overlay.png  | 58×58     | Dark fill mask for cooldown (tile: bottom to top)   |
| Qty_Font.png          | —         | Bitmap font for quantity numbers (Minecraft style)  |

### 7b. Health Bar
**File:** `Assets/Art/UI/HealthBar/`

| File               | Size    | Description                                          |
|--------------------|---------|------------------------------------------------------|
| HealthBar_BG.png   | 320×36  | Background frame for health bar                      |
| HealthBar_Fill.png | 296×20  | Red/green fill bar (slice from left)                 |
| HealthBar_Icon.png | 36×36   | Shushki heart icon on left end                       |

### 7c. Screen Overlays
| File               | Size         | Description                             |
|--------------------|--------------|-----------------------------------------|
| MainMenu_BG.png    | 1920×1080    | Main menu background illustration       |
| PauseOverlay.png   | 1920×1080    | Semi-transparent dark texture overlay   |
| GameOver_Card.png  | 800×400      | "Game Over" card panel                  |
| Victory_Card.png   | 800×400      | "Victory!" card panel, confetti         |

---

## 8. CAMERA SHAKE / SCREEN EFFECTS
These are handled via code — no art needed. Just note for reference:
- Pumpkin Pie: 0.4 magnitude camera shake, 0.5s
- Whale slam: 0.25 magnitude, 0.3s

---

## 9. ANIMATION CONVENTIONS (Unity)

All spritesheets should be:
- **One row**, frames left-to-right
- **Uniform frame size** (state W × state H)
- **PNG with alpha transparency**
- **Texture Type:** Sprite (Multiple) — Unity Sprite Editor to slice

Animator Controller structure for each character:
```
AnyState → Idle (default)
Idle → Walk (isMoving)
Idle → Jump (isGrounded = false)
Walk → Idle (isMoving = false)
Walk → Jump (isGrounded = false)
Jump → Land (isGrounded = true, trigger)
Land → Idle (exit after 0.1s)
Any → Attack (trigger "Attack")
Any → Hurt (trigger "Hurt")
Any → Death (trigger "Death")
```

---

## 10. DELIVERABLE CHECKLIST

```
Assets/Art/
├── Sprites/
│   ├── Player/
│   │   ├── Shushki_Sheet.png      ← Full spritesheet
│   │   └── Shushki_Portrait.png   ← UI portrait
│   ├── Enemies/
│   │   ├── Fish_Sheet.png
│   │   └── Whale_Sheet.png
│   ├── Pies/
│   │   ├── Apple_Sheet.png        ← Fly(4) + Impact(5) + Icon(1)
│   │   ├── Cherry_Sheet.png
│   │   ├── Blueberry_Sheet.png
│   │   ├── LemonMeringue_Sheet.png
│   │   ├── Strawberry_Sheet.png
│   │   ├── Meat_Sheet.png
│   │   ├── Mushroom_Sheet.png
│   │   ├── Pumpkin_Sheet.png
│   │   ├── Chocolate_Sheet.png
│   │   └── Chili_Sheet.png
│   │   └── PieDrop_Sheet.png      ← 10 drop types, 4 frames each
│   └── Background/
│       ├── BG_Sky.png
│       ├── BG_Far.png
│       ├── BG_Mid.png
│       ├── BG_Near.png
│       ├── Ground_Tile.png
│       ├── Ground_Edge_Left.png
│       └── Ground_Edge_Right.png
├── FX/
│   └── StatusEffects/
│       ├── Frozen_Overlay.png
│       ├── Burning_Overlay.png
│       ├── Confused_Overlay.png
│       ├── Slowed_Overlay.png
│       ├── Stunned_Overlay.png
│       └── Electrified_Overlay.png
└── UI/
    ├── Hotbar/
    │   ├── Hotbar_BG.png
    │   ├── Slot_Normal.png
    │   ├── Slot_Selected.png
    │   ├── Slot_Locked.png
    │   ├── Slot_Empty.png
    │   ├── Cooldown_Overlay.png
    │   └── Qty_Font.png
    ├── HealthBar/
    │   ├── HealthBar_BG.png
    │   ├── HealthBar_Fill.png
    │   └── HealthBar_Icon.png
    └── Screens/
        ├── MainMenu_BG.png
        ├── PauseOverlay.png
        ├── GameOver_Card.png
        └── Victory_Card.png
```

---

## PRIORITY ORDER (deliver in this order)

1. **P0 — Shushki (Idle + Walk + Throw)** — needed to validate character feel
2. **P0 — Apple Pie (Fly + Impact + Icon)** — first weapon test
3. **P0 — Fish Enemy (Idle + Walk + Hit + Death)** — first enemy test
4. **P1 — All 10 Pie Icons** — needed for hotbar
5. **P1 — All 10 Pie Fly + Impact** — needed for combat prototype
6. **P1 — Background (Sky + Ground tile)** — polish pass
7. **P1 — Shushki full set** — all animations
8. **P2 — Fish Enemy full set** — all animations
9. **P2 — Whale Boss full set** — boss experience
10. **P2 — Status Effect Overlays** — when base combat is solid
11. **P3 — UI art, Screen overlays** — final polish pass
