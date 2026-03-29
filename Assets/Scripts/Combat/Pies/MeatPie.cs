using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Meat Pie — Heavy hitter. 2× base damage + knockback shockwave nearby.
    /// PLACEHOLDER: brown circle. ARTIST: needs thick Fly + meaty Slam animation.
    /// SFX: heavy thud + ground crack.
    /// </summary>
    public class MeatPie : PieBase
    {
        [Header("Meat — Heavy Impact")]
        [SerializeField] float shockwaveRadius = 1.8f;
        [SerializeField] float shockwaveDamage = 12f;
        [SerializeField] float knockbackForce  = 5f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.55f, 0.25f, 0.1f); // meaty brown
            pieType          = PieType.Meat;
            damage           = 45f;   // 2× baseline
            speed            = 9f;    // slightly slower (it's heavy!)
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            ShockwaveAround();
        }

        protected override void OnImpactGround()
        {
            ShockwaveAround();
        }

        void ShockwaveAround()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                var e    = hit.GetComponent<EnemyBase>();
                var hitRb = hit.GetComponent<Rigidbody2D>();
                e?.TakeDamage(shockwaveDamage);
                if (hitRb != null)
                {
                    Vector2 dir = ((Vector2)(hit.transform.position - transform.position)).normalized;
                    hitRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}
