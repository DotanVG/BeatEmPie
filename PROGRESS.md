# BeatEmPie — Prototype Implementation Progress

> This file lives at the repo root (not in Assets/) and is committed so work can resume after rate-limit interruptions.
> Branch: `claude/game-prototype-assets-bnlSz`
> Last updated: 2026-03-30

---

## Overall Status: 🟡 IN PROGRESS

---

## Phase 1 — PROGRESS.md & Plan ✅
- [x] PROGRESS.md created

---

## Phase 2 — Core Combat Infrastructure ✅
- [x] `StatusEffect.cs` — BeatEmPie namespace
- [x] `PieType.cs` — BeatEmPie namespace
- [x] `StatusEffectHandler.cs` — full implementation (ticks, tints, freeze/stun)
- [x] `EnemyBase.cs` — full state machine, health, damage flash, drop wiring

---

## Phase 3 — Enemy AI ✅
- [x] `FishEnemy.cs` — chase + contact damage
- [x] `WhaleEnemy.cs` — charge windup + slam AOE

---

## Phase 4 — Pie Projectile System ✅
- [x] `PieBase.cs` — physics, trigger collision, Launch API
- [x] `ApplePie.cs` — standard damage
- [x] `CherryPie.cs` — AOE explosion + knockback + stun
- [x] `BlueberryPie.cs` — Frozen status splash
- [x] `LemonMeringuePie.cs` — chain lightning (up to 3 enemies)
- [x] `StrawberryPie.cs` — homing via FixedUpdate
- [x] `MeatPie.cs` — 2× damage + shockwave knockback
- [x] `MushroomPie.cs` — Confused status
- [x] `PumpkinPie.cs` — ultimate AOE + camera shake
- [x] `ChocolatePie.cs` — Slowed + Burning DoT
- [x] `ChiliPie.cs` — Burning + FireZone lingering hazard
- [x] `FireZone.cs` — lingering fire hazard spawned by ChiliPie

---

## Phase 5 — Inventory & Player Combat ✅
- [x] `PieInventory.cs` — quantities, wave unlock progression, cycle, cooldowns, events
- [x] `PlayerCombat.cs` — wired to PieInventory, auto-aim, consume quantities, number keys 1-0

---

## Phase 6 — UI Scripts ✅
- [x] `HealthBar.cs` — Slider + lerp, auto-find PlayerStats
- [x] `PieHUD.cs` — proper UI component wiring stub (superseded by PieHotbar for prototype)
- [x] `GameUI.cs` — OnGUI: main menu, pause, game over, victory, score, wave, whale boss alert
- [x] `PieHotbar.cs` — Minecraft-style 10-slot hotbar with quantities, cooldown, lock states

---

## Phase 7 — Scene Setup ✅
- [x] `GameBootstrapper.cs` — auto-configures entire scene at Play:
  - FishEnemy + WhaleEnemy templates with placeholder sprites
  - 10 pie templates (colored circles)
  - PieDrop template (pickup collectibles)
  - 3 spawn points off-screen
  - UI Canvas with HealthBar + PieHotbar + GameUI
  - ThrowOrigin child on player
  - PieInventory added to player if missing
  - Ground tagging
- [x] Unity MCP not available (Editor not open) — runtime bootstrapper replaces manual wiring

---

## Phase 7b — Minecraft Hotbar + Drop System ✅
- [x] `PieInventory.cs` — wave-unlock progression (waves 1-12), per-type quantities
- [x] `PieDrop.cs` — collectible pickup with bob animation + OnGUI quantity label
- [x] `PieDropTable.cs` — weighted random drops per wave: fish 60% chance, whale always
- [x] `PieHotbar.cs` — 10-slot bar bottom-center: color swatch, cooldown fill, qty badge, lock icon
- [x] `EnemyBase.cs` — SpawnDrops() called on Die(), routes to PieDropTable
- [x] `PlayerCombat.cs` — ConsumePie() on throw; number keys 1-0 select slots

---

## Phase 8 — Artist Spec 🔄 NEXT
- [ ] `ARTIST_SPEC.md` written

---

## Phase 9 — Musician Spec 🔄 NEXT
- [ ] `MUSICIAN_SPEC.md` written

---

## Phase 10 — Commit & Push 🔄 NEXT
- [ ] All changes committed
- [ ] Pushed to `origin claude/game-prototype-assets-bnlSz`

---

## Notes / Blockers

- Unity Editor not running during session → used GameBootstrapper for all scene wiring
- **TO DO on first Unity open**: Tag Shushki prefab as "Player", add GameBootstrapper component
  to GameManager (or any scene object), ensure ground has a Collider2D named "Ground"
- "SpawnPoint" is not a built-in Unity tag — bootstrapper uses name-based search
