using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Chocolate Pie — Slows + DoT. Applies Slowed status and ongoing tick damage.
    /// PLACEHOLDER: dark brown circle. ARTIST: needs Fly + gooey Splat + puddle ground decal.
    /// SFX: wet splat + gooey loop SFX.
    /// </summary>
    public class ChocolatePie : PieBase
    {
        [Header("Chocolate — DoT + Slow")]
        [SerializeField] float slowDuration = 3f;
        [SerializeField] float dotDuration  = 4f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.35f, 0.18f, 0.05f); // dark chocolate brown
            pieType          = PieType.Chocolate;
            damage           = 15f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            enemy.ApplyStatusEffect(StatusEffect.Slowed,  slowDuration);
            enemy.ApplyStatusEffect(StatusEffect.Burning, dotDuration);  // reuse Burning for DoT
        }
    }
}
