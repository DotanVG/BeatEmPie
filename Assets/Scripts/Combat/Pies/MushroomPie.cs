using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Mushroom Pie — Confusion. Enemy turns purple and attacks their own kind.
    /// Confused logic: target = nearest OTHER enemy instead of player.
    /// PLACEHOLDER: purple circle. ARTIST: needs spiraling mushroom Splat + question-mark FX.
    /// SFX: cartoony boing + wobbly confusion loop on enemy.
    /// </summary>
    public class MushroomPie : PieBase
    {
        [Header("Mushroom — Confusion")]
        [SerializeField] float confusionDuration = 4f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.6f, 0.2f, 0.8f); // mushroom purple
            pieType          = PieType.Mushroom;
            damage           = 10f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            enemy.ApplyStatusEffect(StatusEffect.Confused, confusionDuration);
            // ConfusedEnemy script (on prefab) watches for this status and redirects AI
        }
    }
}
