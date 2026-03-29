using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Cherry Pie — Explosive AOE. Blast radius damages + stuns nearby enemies.
    /// PLACEHOLDER: red circle. ARTIST: needs Fly/Spin + big Explosion burst animation.
    /// SFX: large boom. Screen shake on impact.
    /// </summary>
    public class CherryPie : PieBase
    {
        [Header("Cherry — Explosion")]
        [SerializeField] float explosionRadius = 2.5f;
        [SerializeField] float explosionDamage = 15f;
        [SerializeField] float stunDuration    = 0.8f;
        [SerializeField] float knockbackForce  = 6f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.9f, 0.1f, 0.15f); // cherry red
            pieType          = PieType.Cherry;
            damage           = 25f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            Explode();
        }

        protected override void OnImpactGround()
        {
            Explode();
        }

        void Explode()
        {
            // AOE splash
            SplashDamage(explosionRadius, explosionDamage, StatusEffect.Stunned, stunDuration);

            // Knockback all enemies in radius
            var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                var hitRb = hit.GetComponent<Rigidbody2D>();
                if (hitRb == null) continue;
                Vector2 dir = ((Vector2)(hit.transform.position - transform.position)).normalized;
                hitRb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
