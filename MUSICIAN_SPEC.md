# BeatEmPie — Musician Specification

> Game: 2D side-scrolling beat-em-up. Platform: PC/Mac/WebGL.
> Engine: Unity 6. Audio: Unity AudioManager with crossfade between tracks.
> Format: OGG Vorbis (primary), WAV fallback. Stereo. 44.1 kHz.
> Style direction: **Quirky, bouncy, slightly chaotic pastry-themed — cartoony orchestral
> meets jaunty tavern music. Think: Cuphead energy with a bakery twist.**

---

## PART 1 — MUSIC TRACKS

Existing generated placeholder tracks are in `Assets/Audio/Music/`. Replace them.

### Music System Overview
The game has a `DynamicMusicController` that crossfades between tracks automatically:
- `< 5 enemies` → **Gameplay Calm**
- `≥ 5 enemies` → **Gameplay Intense**
- Whale spawns → **Boss Fight** (immediate crossfade, ~1s)
- Wave cleared → returns to **Gameplay Calm**

All gameplay tracks **must loop seamlessly** at a consistent tempo so crossfades work.
BPM consistency across all gameplay tracks is preferred (e.g., all at 128 BPM, different intensities).

---

### Track 1: Main Menu — `bgm_mainmenu.wav`
**Duration:** 1:30–2:00 (looping)
**Feel:** Whimsical, inviting, slightly mysterious bakery vibe.
Imagine floating through a magical pie shop before a big adventure.
- Soft accordion, light strings, gentle harp arpeggios
- Warm, welcoming, slightly silly
- Fades to quiet on menu interaction

---

### Track 2: Gameplay Calm — `bgm_gameplay_calm.wav`
**Duration:** 1:30–2:30 (seamless loop at loop point)
**Feel:** Bouncy, upbeat, light combat energy. Pie fight music.
- Ukulele or banjo + light drums + whistling melody
- Playful and driving but not overwhelming
- The "default" battle state — still calm enough to read clearly

---

### Track 3: Gameplay Intense — `bgm_gameplay_intense.wav`
**Duration:** 1:30–2:30 (seamless loop — same tempo as Calm)
**Feel:** Same tempo as Calm but ramped up — more percussion, brass stabs, urgency.
- Brass section hits, snare rolls, driving bass line
- Feels like a kitchen battle getting out of control
- Must crossfade cleanly from/to Calm track (same BPM)

---

### Track 4: Boss Fight — `bgm_boss.wav`
**Duration:** 2:00–3:00 (seamless loop)
**Feel:** DRAMATIC. Giant whale has entered. Full orchestra moment.
- Huge brass hits, tuba rumble, rising strings, battle drums
- Operatic or orchestral — over-the-top grandiosity
- Think: "a whale in a top hat vs. a girl with pies"
- Build from quiet intro to full intensity quickly (8-bar intro max)

---

### Track 5: Victory — `bgm_victory.wav`
**Duration:** 0:20–0:45 (non-looping, plays once)
**Feel:** Triumphant fanfare, confetti, "you did it!" energy.
- Short brass fanfare + percussion
- Big satisfying resolution chord at end
- Can end naturally (no loop)

---

### Track 6: Game Over — `bgm_gameover.wav`
**Duration:** 0:15–0:30 (non-looping, plays once)
**Feel:** Comedic sad trombone moment, not crushing — just goofy defeat.
- Wah-wah brass descend
- Maybe a little pie splat sound woven in musically
- Leaves space for player to click "Play Again"

---

## PART 2 — SOUND EFFECTS

File location: `Assets/Audio/SFX/`
All SFX: **Mono preferred** (Unity handles 3D panning), 44.1 kHz, short duration.

The `SFXLibrary.cs` ScriptableObject holds references to all clips.
Fields are defined there — match file names below.

---

### 2a. PIE THROWS & IMPACTS

#### Generic (fallback)
| File                      | Duration | Description                                            |
|---------------------------|----------|--------------------------------------------------------|
| `sfx_pie_throw.wav`       | 0.3s     | Quick whoosh — pie leaves hand                         |
| `sfx_pie_impact.wav`      | 0.4s     | Generic wet splat on impact                            |

#### Per-Pie Impacts (wired to `SFXLibrary.GetPieImpact(PieType)`)
| File                           | Duration | Description                                          |
|--------------------------------|----------|------------------------------------------------------|
| `sfx_pie_apple.wav`            | 0.3s     | Satisfying thwack — crisp apple crunch               |
| `sfx_pie_cherry.wav`           | 0.6s     | **BOOM** — big cartoon explosion                     |
| `sfx_pie_blueberry.wav`        | 0.5s     | Freeze whoosh + ice crack shard sound                |
| `sfx_pie_lemon_meringue.wav`   | 0.5s     | Electric ZAP + crackling arc                         |
| `sfx_pie_strawberry.wav`       | 0.4s     | Cute heart pop + swoosh                              |
| `sfx_pie_meat.wav`             | 0.5s     | THUD — deep heavy impact + floor crack               |
| `sfx_pie_mushroom.wav`         | 0.5s     | Cartoony BOING + mushroom pop                        |
| `sfx_pie_pumpkin.wav`          | 0.8s     | MASSIVE BOOM — bass rumble + echo                    |
| `sfx_pie_chocolate.wav`        | 0.4s     | Wet gooey splat + drip                               |
| `sfx_pie_chili.wav`            | 0.5s     | Sizzle + fire whoosh + crackle                       |

#### Fire Zone (ChiliPie lingering fire)
| File                           | Duration | Description                                          |
|--------------------------------|----------|------------------------------------------------------|
| `sfx_firezone_loop.wav`        | 1.0s     | Short looping crackle — used on FireZone AudioSource |

---

### 2b. ENEMY SOUNDS

| File                      | Duration | Description                                                  |
|---------------------------|----------|--------------------------------------------------------------|
| `sfx_enemy_hit.wav`       | 0.3s     | Generic enemy hurt grunt/squish                              |
| `sfx_enemy_death.wav`     | 0.5s     | Comedic enemy death — pop + splat                            |
| `sfx_fish_chomp.wav`      | 0.3s     | Fish bite/chomp sound for attack                             |
| `sfx_whale_roar.wav`      | 1.0s     | Whale boss entrance roar — deep, resonant, funny             |
| `sfx_whale_charge.wav`    | 0.5s     | Charge-up whoosh / steam blowhole                            |
| `sfx_whale_slam.wav`      | 0.8s     | Whale lands — massive thud + shockwave boom                  |
| `sfx_whale_death.wav`     | 1.5s     | Epic whale death — long splash, gurgle, tiny toot at end     |

---

### 2c. PLAYER SOUNDS

| File                      | Duration | Description                                                  |
|---------------------------|----------|--------------------------------------------------------------|
| `sfx_player_hurt.wav`     | 0.3s     | Shushki ouch sound — cute yelp                               |
| `sfx_player_death.wav`    | 0.8s     | Shushki death — sad exclamation + fade                       |
| `sfx_player_jump.wav`     | 0.2s     | Light hop sound — springy                                    |
| `sfx_player_land.wav`     | 0.2s     | Soft landing pat                                             |

---

### 2d. PIE DROP COLLECTIBLES

| File                        | Duration | Description                                                |
|-----------------------------|----------|------------------------------------------------------------|
| `sfx_pie_drop_spawn.wav`    | 0.3s     | Twinkle sound when pie drop appears                        |
| `sfx_pie_pickup.wav`        | 0.3s     | Satisfying pickup chime — "bling!"                         |
| `sfx_pie_pickup_rare.wav`   | 0.5s     | Fancier version for rare pies (Pumpkin, Chili, Chocolate)  |

---

### 2e. UI / SYSTEM SOUNDS

| File                      | Duration | Description                                                  |
|---------------------------|----------|--------------------------------------------------------------|
| `sfx_ui_select.wav`       | 0.1s     | Hotbar slot cycle click                                      |
| `sfx_ui_confirm.wav`      | 0.2s     | Menu button confirm                                          |
| `sfx_wave_start.wav`      | 0.5s     | Wave begins — short dramatic hit                             |
| `sfx_wave_complete.wav`   | 0.8s     | Wave cleared — little fanfare jingle                         |
| `sfx_pie_unlock.wav`      | 0.6s     | New pie type unlocked — discovery jingle                     |
| `sfx_game_start.wav`      | 0.3s     | Countdown "go" sound when game starts                        |

---

## PART 3 — AUDIO DESIGN NOTES

### Crossfade Architecture
The `AudioManager` crossfades music tracks via coroutine (default 1.5s). All gameplay
tracks **must share a consistent BPM** so the musical feel matches across transitions.
Recommended BPM: **128 BPM** (Calm, Intense, Boss should all sync at this tempo).

### SFX Mixing Guidelines
- Pie impacts: **-3 dB to -6 dB** (shouldn't overpower music)
- Enemy hits: **-6 dB**
- Whale slam / Cherry explosion: **-1 dB** (these are "impact moments" — they should punch)
- UI sounds: **-12 dB** (subtle, never distracting)
- Player hurt: **-3 dB** (player needs to hear this clearly)

### Placeholder Tracks (already exist)
The existing procedurally generated WAV files in `Assets/Audio/Music/` are placeholder.
They are referenced by name — replace the files with the same names and Unity
will auto-swap them via the asset database.

---

## PART 4 — DELIVERABLE CHECKLIST

```
Assets/Audio/
├── Music/
│   ├── bgm_mainmenu.wav         ← REPLACE (placeholder exists)
│   ├── bgm_gameplay_calm.wav    ← REPLACE (placeholder exists)
│   ├── bgm_gameplay_intense.wav ← REPLACE (placeholder exists)
│   ├── bgm_boss.wav             ← REPLACE (placeholder exists)
│   ├── bgm_victory.wav          ← REPLACE (placeholder exists)
│   └── bgm_gameover.wav         ← REPLACE (placeholder exists)
└── SFX/
    ├── Pies/
    │   ├── sfx_pie_throw.wav
    │   ├── sfx_pie_impact.wav
    │   ├── sfx_pie_apple.wav
    │   ├── sfx_pie_cherry.wav
    │   ├── sfx_pie_blueberry.wav
    │   ├── sfx_pie_lemon_meringue.wav
    │   ├── sfx_pie_strawberry.wav
    │   ├── sfx_pie_meat.wav
    │   ├── sfx_pie_mushroom.wav
    │   ├── sfx_pie_pumpkin.wav
    │   ├── sfx_pie_chocolate.wav
    │   ├── sfx_pie_chili.wav
    │   └── sfx_firezone_loop.wav
    ├── Enemies/
    │   ├── sfx_enemy_hit.wav
    │   ├── sfx_enemy_death.wav
    │   ├── sfx_fish_chomp.wav
    │   ├── sfx_whale_roar.wav
    │   ├── sfx_whale_charge.wav
    │   ├── sfx_whale_slam.wav
    │   └── sfx_whale_death.wav
    ├── Player/
    │   ├── sfx_player_hurt.wav
    │   ├── sfx_player_death.wav
    │   ├── sfx_player_jump.wav
    │   └── sfx_player_land.wav
    ├── Pickups/
    │   ├── sfx_pie_drop_spawn.wav
    │   ├── sfx_pie_pickup.wav
    │   └── sfx_pie_pickup_rare.wav
    └── UI/
        ├── sfx_ui_select.wav
        ├── sfx_ui_confirm.wav
        ├── sfx_wave_start.wav
        ├── sfx_wave_complete.wav
        ├── sfx_pie_unlock.wav
        └── sfx_game_start.wav
```

---

## PRIORITY ORDER (deliver in this order)

1. **P0** — `bgm_gameplay_calm.wav` — needed to validate audio feel immediately
2. **P0** — `sfx_pie_throw.wav` + `sfx_pie_impact.wav` — core game loop feedback
3. **P0** — `sfx_player_hurt.wav` + `sfx_enemy_hit.wav` — combat feedback
4. **P1** — `bgm_gameplay_intense.wav` + `bgm_boss.wav` — dynamic music system test
5. **P1** — All 10 pie-specific impacts — polish pass
6. **P1** — `sfx_pie_pickup.wav` + `sfx_pie_unlock.wav` — progression feel
7. **P2** — `bgm_mainmenu.wav` + `bgm_victory.wav` + `bgm_gameover.wav`
8. **P2** — All enemy and whale sounds
9. **P3** — UI sounds, wave start/complete, fire zone loop
