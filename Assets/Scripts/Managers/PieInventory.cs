using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Tracks which pies are unlocked, which is selected, and manages per-type cooldowns.
    /// Attach alongside PlayerCombat on the Shushki prefab.
    /// </summary>
    public class PieInventory : MonoBehaviour
    {
        // Cooldown durations per pie type (seconds)
        static readonly Dictionary<PieType, float> CooldownDurations = new()
        {
            { PieType.Apple,         0.5f },
            { PieType.Cherry,        3.0f },
            { PieType.Blueberry,     2.0f },
            { PieType.LemonMeringue, 2.5f },
            { PieType.Strawberry,    2.0f },
            { PieType.Meat,          1.5f },
            { PieType.Mushroom,      2.5f },
            { PieType.Pumpkin,       8.0f },
            { PieType.Chocolate,     2.0f },
            { PieType.Chili,         2.0f },
        };

        [Header("Unlocked Pies — edit in Inspector to unlock more")]
        [SerializeField] List<PieType> unlockedPies = new() { PieType.Apple };

        readonly Dictionary<PieType, float> cooldownTimers = new();
        int currentIndex;

        public event Action OnPieChanged;

        // ── Public API ────────────────────────────────────────────────────

        public PieType GetCurrentPie() =>
            unlockedPies.Count > 0 ? unlockedPies[currentIndex] : PieType.Apple;

        public int GetCurrentIndex() => currentIndex;

        public List<PieType> GetUnlockedPies() => unlockedPies;

        public void CycleNext()
        {
            if (unlockedPies.Count <= 1) return;
            currentIndex = (currentIndex + 1) % unlockedPies.Count;
            OnPieChanged?.Invoke();
        }

        public void CyclePrev()
        {
            if (unlockedPies.Count <= 1) return;
            currentIndex = (currentIndex - 1 + unlockedPies.Count) % unlockedPies.Count;
            OnPieChanged?.Invoke();
        }

        public bool IsOnCooldown(PieType type) =>
            cooldownTimers.TryGetValue(type, out float t) && t > 0f;

        public float GetCooldownRemaining(PieType type) =>
            cooldownTimers.TryGetValue(type, out float t) ? Mathf.Max(0f, t) : 0f;

        public float GetCooldownDuration(PieType type) =>
            CooldownDurations.TryGetValue(type, out float d) ? d : 1f;

        public void TriggerCooldown(PieType type)
        {
            cooldownTimers[type] = GetCooldownDuration(type);
        }

        public void UnlockPie(PieType type)
        {
            if (!unlockedPies.Contains(type))
                unlockedPies.Add(type);
        }

        // ── Update ────────────────────────────────────────────────────────

        void Update()
        {
            var keys = new List<PieType>(cooldownTimers.Keys);
            foreach (var key in keys)
                cooldownTimers[key] = Mathf.Max(0f, cooldownTimers[key] - Time.deltaTime);
        }
    }
}
