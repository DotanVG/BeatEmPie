# BeatEmPie — Prototype Implementation Progress

> This file lives at the repo root (not in Assets/) and is committed so work can resume after rate-limit interruptions.
> Branch: `claude/game-prototype-assets-bnlSz`
> Last updated: 2026-03-29

---

## Overall Status: 🟡 IN PROGRESS

---

## Phase 1 — PROGRESS.md & Plan
- [x] PROGRESS.md created

---

## Phase 2 — Core Combat Infrastructure (Scripts)
- [ ] `StatusEffect.cs` — add BeatEmPie namespace
- [ ] `PieType.cs` — add BeatEmPie namespace
- [ ] `StatusEffectHandler.cs` — full implementation
- [ ] `EnemyBase.cs` — full state machine (Idle/Chase/Attack/Stunned/Dead), health, damage flash, namespace

---

## Phase 3 — Enemy AI
- [ ] `FishEnemy.cs` — chase player, contact damage, flip sprite
- [ ] `WhaleEnemy.cs` — boss: slow walk, charge windup, slam AOE

---

## Phase 4 — Pie Projectile System
- [ ] `PieBase.cs` — abstract projectile: velocity, trigger collision, lifetime, namespace
- [ ] `ApplePie.cs` — standard damage only
- [ ] `CherryPie.cs` — AOE explosion (OverlapCircle splash)
- [ ] `BlueberryPie.cs` — Frozen status effect
- [ ] `LemonMeringuePie.cs` — chain lightning (hit up to 3 nearby enemies)
- [ ] `StrawberryPie.cs` — homing (track nearest enemy)
- [ ] `MeatPie.cs` — 2× damage, knockback
- [ ] `MushroomPie.cs` — Confused status (enemy targets allies)
- [ ] `PumpkinPie.cs` — large AOE ultimate
- [ ] `ChocolatePie.cs` — Slowed + DoT (Burning-like)
- [ ] `ChiliPie.cs` — Burning DoT fire trail

---

## Phase 5 — Inventory & Player Combat
- [ ] `PieInventory.cs` — full: unlock list, cooldown timers, cycle, events
- [ ] `PlayerCombat.cs` — wire to PieInventory, spawn pie prefab at throwOrigin toward nearest enemy

---

## Phase 6 — UI Scripts
- [ ] `HealthBar.cs` — UnityUI Slider + lerp, auto-find PlayerStats
- [ ] `PieHUD.cs` — current pie text/icon, cooldown radial, auto-find PieInventory
- [ ] `GameUI.cs` — OnGUI overlay for MainMenu / GameOver / Victory / score / wave

---

## Phase 7 — Scene Setup via Unity MCP
- [ ] Verify scene root objects
- [ ] Create FishEnemy prefab (BoxCollider2D, Rigidbody2D, FishEnemy script, placeholder sprite)
- [ ] Create WhaleEnemy prefab (larger, WhaleEnemy script)
- [ ] Create 10 pie prefabs (CircleCollider2D trigger, Rigidbody2D, per-pie script)
- [ ] Add 2–3 spawn points (empty GameObjects on screen edges)
- [ ] Create UI Canvas (Screen Space Overlay, 1920×1080 reference)
- [ ] Add HealthBar (Slider) to Canvas
- [ ] Add PieHUD (Text + Image) to Canvas
- [ ] Add GameUI component to Canvas
- [ ] Wire EnemySpawner inspector refs (fish/whale prefabs, spawn points)
- [ ] Wire PlayerCombat inspector refs (throwOrigin, pie prefabs array)
- [ ] Tag Player as "Player", Ground as "Ground", Enemies as "Enemy"

---

## Phase 8 — Artist Spec
- [ ] `ARTIST_SPEC.md` written (player, enemies, pies, FX, UI, BG)

---

## Phase 9 — Musician Spec
- [ ] `MUSICIAN_SPEC.md` written (music tracks, SFX list with descriptions)

---

## Phase 10 — Commit & Push
- [ ] All changes committed with emoji-prefix message
- [ ] Pushed to `origin claude/game-prototype-assets-bnlSz`

---

## Notes / Blockers

_None yet_
