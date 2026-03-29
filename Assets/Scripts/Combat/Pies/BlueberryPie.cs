using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Blueberry Pie — Freeze. Applies Frozen status to hit enemy.
    /// PLACEHOLDER: icy blue circle. ARTIST: needs Fly + ice-crystal Splat animation.
    /// SFX: whoosh + freeze/crack sound.
    /// </summary>
    public class BlueberryPie : PieBase
    {
        [Header("Blueberry — Freeze")]
        [SerializeField] float freezeDuration = 2.5f;
        [SerializeField] float freezeRadius   = 1.2f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.3f, 0.35f, 0.95f); // blueberry blue
            pieType          = PieType.Blueberry;
            damage           = 15f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            // Freeze the hit enemy + any enemy in small splash radius
            SplashDamage(freezeRadius, 0f, StatusEffect.Frozen, freezeDuration);
            enemy.ApplyStatusEffect(StatusEffect.Frozen, freezeDuration);
        }
    }
}
