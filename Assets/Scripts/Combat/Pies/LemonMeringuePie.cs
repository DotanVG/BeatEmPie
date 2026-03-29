using System.Collections.Generic;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Lemon Meringue Pie — Chain Lightning. Zaps up to N nearby enemies.
    /// PLACEHOLDER: bright yellow circle. ARTIST: needs Fly + electric arc animations between enemies.
    /// SFX: electric zap/crackle for each chain jump.
    /// </summary>
    public class LemonMeringuePie : PieBase
    {
        [Header("Lemon Meringue — Chain Lightning")]
        [SerializeField] int   maxChainTargets  = 3;
        [SerializeField] float chainRadius      = 3.5f;
        [SerializeField] float chainDamage      = 12f;
        [SerializeField] float electrifyDuration = 1.5f;

        protected override void Awake()
        {
            placeholderColor = new Color(1f, 0.95f, 0.1f); // electric yellow
            pieType          = PieType.LemonMeringue;
            damage           = 20f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase firstEnemy)
        {
            firstEnemy.ApplyStatusEffect(StatusEffect.Electrified, electrifyDuration);

            // Chain to nearby enemies
            var chained = new HashSet<EnemyBase> { firstEnemy };
            Vector3 origin = firstEnemy.transform.position;

            for (int i = 0; i < maxChainTargets - 1; i++)
            {
                var hits = Physics2D.OverlapCircleAll(origin, chainRadius);
                EnemyBase next = null;
                float bestDist = float.MaxValue;

                foreach (var hit in hits)
                {
                    if (!hit.CompareTag("Enemy")) continue;
                    if (!hit.TryGetComponent<EnemyBase>(out var e)) continue;
                    if (chained.Contains(e)) continue;
                    float d = Vector2.Distance(origin, hit.transform.position);
                    if (d < bestDist) { bestDist = d; next = e; }
                }

                if (next == null) break;
                next.TakeDamage(chainDamage);
                next.ApplyStatusEffect(StatusEffect.Electrified, electrifyDuration);
                chained.Add(next);
                origin = next.transform.position;
            }
        }
    }
}
