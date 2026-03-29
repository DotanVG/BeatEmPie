using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Chili Pie — Burns! Applies Burning DoT (8 dps via StatusEffectHandler) on direct hit.
    /// Also spawns a fire zone at impact point that lingers and burns other enemies.
    /// PLACEHOLDER: orange-red circle. ARTIST: needs Fly + flame trail + Fire Zone ground sprite.
    /// SFX: sizzle/crackling fire on hit; fire zone loop ambience.
    /// </summary>
    public class ChiliPie : PieBase
    {
        [Header("Chili — Fire")]
        [SerializeField] float burnDuration  = 3.5f;
        [SerializeField] float zoneRadius    = 1.5f;
        [SerializeField] float zoneDuration  = 4f;

        protected override void Awake()
        {
            placeholderColor = new Color(1f, 0.3f, 0.0f); // chili orange-red
            pieType          = PieType.Chili;
            damage           = 18f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            enemy.ApplyStatusEffect(StatusEffect.Burning, burnDuration);
            SpawnFireZone();
        }

        protected override void OnImpactGround()
        {
            SpawnFireZone();
        }

        void SpawnFireZone()
        {
            // Create a lingering fire zone at the impact point
            var zone = new GameObject("FireZone");
            zone.transform.position = transform.position;
            zone.AddComponent<FireZone>().Init(zoneRadius, zoneDuration, burnDuration);
        }
    }
}
