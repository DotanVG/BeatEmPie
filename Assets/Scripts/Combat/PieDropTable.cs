using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Controls what pies drop from enemies when they die, based on current wave.
    ///
    /// DROP DESIGN RULES:
    ///   - Common pies (Apple, Cherry, Blueberry) drop in higher quantities (5-10)
    ///   - Mid-tier pies (LemonMeringue, Strawberry, Meat, Mushroom) drop 2-5
    ///   - Rare pies (Pumpkin, Chocolate, Chili) drop 1-2 only
    ///   - Only pies that match the current wave unlock tier can drop
    ///   - Fish enemies: 60% chance to drop, 1 pie type from tier
    ///   - Whale boss: always drops, 3-5 pie types, guaranteed rare
    ///
    /// Called from EnemyBase.Die() after spawning enemy death.
    /// A PieDrop prefab is required — built by GameBootstrapper at runtime.
    /// </summary>
    public static class PieDropTable
    {
        // ── Drop entry ────────────────────────────────────────────────────

        struct DropEntry
        {
            public PieType Type;
            public int     MinQty;
            public int     MaxQty;
            public int     MinWave;   // only drops from this wave onwards
            public float   Weight;    // relative probability weight
        }

        static readonly DropEntry[] Table =
        {
            new() { Type = PieType.Apple,         MinQty = 5,  MaxQty = 10, MinWave = 1,  Weight = 50f },
            new() { Type = PieType.Cherry,         MinQty = 4,  MaxQty = 8,  MinWave = 2,  Weight = 30f },
            new() { Type = PieType.Blueberry,      MinQty = 3,  MaxQty = 7,  MinWave = 3,  Weight = 25f },
            new() { Type = PieType.LemonMeringue,  MinQty = 2,  MaxQty = 5,  MinWave = 4,  Weight = 18f },
            new() { Type = PieType.Strawberry,     MinQty = 2,  MaxQty = 5,  MinWave = 5,  Weight = 18f },
            new() { Type = PieType.Meat,           MinQty = 2,  MaxQty = 4,  MinWave = 6,  Weight = 15f },
            new() { Type = PieType.Mushroom,       MinQty = 2,  MaxQty = 4,  MinWave = 7,  Weight = 12f },
            new() { Type = PieType.Pumpkin,        MinQty = 1,  MaxQty = 2,  MinWave = 8,  Weight = 5f  },
            new() { Type = PieType.Chocolate,      MinQty = 2,  MaxQty = 3,  MinWave = 10, Weight = 10f },
            new() { Type = PieType.Chili,          MinQty = 1,  MaxQty = 3,  MinWave = 12, Weight = 8f  },
        };

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Roll a drop for a fish enemy. Returns null if no drop this time.
        /// </summary>
        public static DropResult? RollFishDrop(int currentWave)
        {
            if (Random.value > 0.6f) return null;   // 60% drop chance
            return RollOne(currentWave);
        }

        /// <summary>
        /// Roll drops for the whale boss — always drops, returns 3-5 results.
        /// </summary>
        public static DropResult[] RollWhaleDrop(int currentWave)
        {
            int count   = Random.Range(3, 6);
            var results = new DropResult[count];
            // Guarantee at least one rare pie
            results[0] = RollRare(currentWave);
            for (int i = 1; i < count; i++)
                results[i] = RollOne(currentWave);
            return results;
        }

        // ── Internal ──────────────────────────────────────────────────────

        static DropResult RollOne(int wave)
        {
            float totalWeight = 0f;
            foreach (var e in Table)
            {
                if (e.MinWave <= wave) totalWeight += e.Weight;
            }

            float roll = Random.Range(0f, totalWeight);
            float acc  = 0f;
            foreach (var e in Table)
            {
                if (e.MinWave > wave) continue;
                acc += e.Weight;
                if (roll <= acc)
                    return new DropResult { Type = e.Type, Qty = Random.Range(e.MinQty, e.MaxQty + 1) };
            }
            return new DropResult { Type = PieType.Apple, Qty = 3 };
        }

        static DropResult RollRare(int wave)
        {
            // Roll from Pumpkin, Chocolate, Chili only if available; else fall back to mid-tier
            DropEntry[] rares =
            {
                new() { Type = PieType.Pumpkin,   MinQty = 1, MaxQty = 2,  MinWave = 8  },
                new() { Type = PieType.Chocolate, MinQty = 2, MaxQty = 3,  MinWave = 10 },
                new() { Type = PieType.Chili,     MinQty = 1, MaxQty = 2,  MinWave = 12 },
                new() { Type = PieType.Mushroom,  MinQty = 2, MaxQty = 4,  MinWave = 7  },
                new() { Type = PieType.Pumpkin,   MinQty = 1, MaxQty = 2,  MinWave = 8  },
            };

            foreach (var r in rares)
            {
                if (r.MinWave <= wave)
                    return new DropResult { Type = r.Type, Qty = Random.Range(r.MinQty, r.MaxQty + 1) };
            }
            return RollOne(wave);
        }
    }

    public struct DropResult
    {
        public PieType Type;
        public int     Qty;
    }
}
