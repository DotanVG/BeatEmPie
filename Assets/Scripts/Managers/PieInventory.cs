using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Tracks which pies are unlocked, per-type quantities (like Minecraft stack counts),
    /// selected hotbar slot, and cooldown timers.
    ///
    /// UNLOCK PROGRESSION (by wave — wired via EnemySpawner events):
    ///   Wave  1 → Apple (always available, starts with infinite supply)
    ///   Wave  2 → Cherry unlocked
    ///   Wave  3 → Blueberry unlocked
    ///   Wave  4 → LemonMeringue unlocked
    ///   Wave  5 → Strawberry unlocked
    ///   Wave  6 → Meat unlocked
    ///   Wave  7 → Mushroom unlocked
    ///   Wave  8 → Pumpkin unlocked
    ///   Wave 10 → Chocolate unlocked
    ///   Wave 12 → Chili unlocked
    ///
    /// QUANTITY MODEL:
    ///   - Apple: unlimited (int.MaxValue sentinel)
    ///   - Others: start at 0; gained by picking up PieDrop collectibles on the map
    ///   - Can't throw a pie with 0 quantity (locked/empty slot shows greyed out)
    ///   - Drops from enemies get more abundant later but rarer pies drop in smaller amounts
    /// </summary>
    public class PieInventory : MonoBehaviour
    {
        public const int Unlimited = int.MaxValue;  // sentinel for Apple

        // ── Wave unlock thresholds ────────────────────────────────────────

        static readonly Dictionary<PieType, int> UnlockWave = new()
        {
            { PieType.Apple,          1  },
            { PieType.Cherry,         2  },
            { PieType.Blueberry,      3  },
            { PieType.LemonMeringue,  4  },
            { PieType.Strawberry,     5  },
            { PieType.Meat,           6  },
            { PieType.Mushroom,       7  },
            { PieType.Pumpkin,        8  },
            { PieType.Chocolate,      10 },
            { PieType.Chili,          12 },
        };

        // ── Cooldown durations ────────────────────────────────────────────

        static readonly Dictionary<PieType, float> CooldownDurations = new()
        {
            { PieType.Apple,          0.5f },
            { PieType.Cherry,         3.0f },
            { PieType.Blueberry,      2.0f },
            { PieType.LemonMeringue,  2.5f },
            { PieType.Strawberry,     2.0f },
            { PieType.Meat,           1.5f },
            { PieType.Mushroom,       2.5f },
            { PieType.Pumpkin,        8.0f },
            { PieType.Chocolate,      2.0f },
            { PieType.Chili,          2.0f },
        };

        // ── State ─────────────────────────────────────────────────────────

        // Quantities — Apple = Unlimited, others = 0 until picked up
        readonly Dictionary<PieType, int>   quantities     = new();
        readonly Dictionary<PieType, float> cooldownTimers = new();
        readonly HashSet<PieType>           unlockedPies   = new();

        // Hotbar slot order always = PieType enum order (0..9)
        int selectedSlot = 0;  // 0 = Apple

        public event Action OnInventoryChanged;   // fired on quantity, unlock, or slot change

        // ── Lifecycle ─────────────────────────────────────────────────────

        void Awake()
        {
            // Apple always unlocked + unlimited
            UnlockPie(PieType.Apple);
            quantities[PieType.Apple] = Unlimited;

            // Subscribe to wave events for progressive unlocks
            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.OnWaveStarted += OnWaveStarted;
        }

        void OnEnable()
        {
            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.OnWaveStarted += OnWaveStarted;
        }

        void OnDisable()
        {
            if (EnemySpawner.Instance != null)
                EnemySpawner.Instance.OnWaveStarted -= OnWaveStarted;
        }

        // ── Public API — slots ────────────────────────────────────────────

        public int       SelectedSlot    => selectedSlot;
        public PieType   SelectedPie     => (PieType)selectedSlot;

        public void SelectSlot(int slot)
        {
            if (slot < 0 || slot > 9) return;
            selectedSlot = slot;
            OnInventoryChanged?.Invoke();
        }

        public void CycleNext()
        {
            int next = (selectedSlot + 1) % 10;
            selectedSlot = next;
            OnInventoryChanged?.Invoke();
        }

        public void CyclePrev()
        {
            int prev = (selectedSlot - 1 + 10) % 10;
            selectedSlot = prev;
            OnInventoryChanged?.Invoke();
        }

        // ── Public API — unlock / quantity ────────────────────────────────

        public bool IsUnlocked(PieType type)  => unlockedPies.Contains(type);

        public int GetQuantity(PieType type)
        {
            quantities.TryGetValue(type, out int q);
            return q;
        }

        public bool HasAmmo(PieType type)
        {
            if (!IsUnlocked(type)) return false;
            return GetQuantity(type) > 0;
        }

        /// <summary>Add qty pies of type. Also unlocks the type if not yet unlocked.</summary>
        public void AddPies(PieType type, int qty)
        {
            if (!IsUnlocked(type)) UnlockPie(type);

            if (quantities.TryGetValue(type, out int current))
                quantities[type] = current == Unlimited ? Unlimited : Mathf.Min(current + qty, 99);
            else
                quantities[type] = qty;

            OnInventoryChanged?.Invoke();
        }

        /// <summary>Consume one pie. Returns false if out of ammo.</summary>
        public bool ConsumePie(PieType type)
        {
            if (!HasAmmo(type)) return false;
            if (quantities[type] != Unlimited)
                quantities[type]--;
            TriggerCooldown(type);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public void UnlockPie(PieType type)
        {
            if (unlockedPies.Add(type))
            {
                if (!quantities.ContainsKey(type)) quantities[type] = 0;
                OnInventoryChanged?.Invoke();
            }
        }

        // ── Public API — cooldowns ────────────────────────────────────────

        public bool  IsOnCooldown(PieType type) =>
            cooldownTimers.TryGetValue(type, out float t) && t > 0f;

        public float GetCooldownRemaining(PieType type) =>
            cooldownTimers.TryGetValue(type, out float t) ? Mathf.Max(0f, t) : 0f;

        public float GetCooldownDuration(PieType type) =>
            CooldownDurations.TryGetValue(type, out float d) ? d : 1f;

        void TriggerCooldown(PieType type) =>
            cooldownTimers[type] = GetCooldownDuration(type);

        // ── Wave-based unlock progression ─────────────────────────────────

        void OnWaveStarted(int wave)
        {
            foreach (var kv in UnlockWave)
            {
                if (wave >= kv.Value && !IsUnlocked(kv.Key))
                    UnlockPie(kv.Key);
            }
        }

        // ── Legacy compatibility (used by PlayerCombat + PieHUD) ──────────

        public PieType   GetCurrentPie()     => SelectedPie;
        public int       GetCurrentIndex()   => selectedSlot;

        // ── Update ────────────────────────────────────────────────────────

        void Update()
        {
            var keys = new List<PieType>(cooldownTimers.Keys);
            foreach (var key in keys)
                cooldownTimers[key] = Mathf.Max(0f, cooldownTimers[key] - Time.deltaTime);
        }
    }
}
