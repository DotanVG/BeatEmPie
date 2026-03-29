using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Whale Enemy — Boss tier. Slow walk, then a telegraphed charge slam.
    /// PLACEHOLDER: large purple rectangle. Replace with Whale spritesheet per ARTIST_SPEC.md.
    /// Animation states needed: Idle, Walk, ChargeWindup, Charge, Slam, Hit, Death.
    /// </summary>
    public class WhaleEnemy : EnemyBase
    {
        [Header("Whale Settings")]
        [SerializeField] float moveSpeed       = 1.4f;
        [SerializeField] float chargeSpeed     = 7f;
        [SerializeField] float chargeRange     = 5f;
        [SerializeField] float chargeWindup    = 1.2f;   // seconds of telegraph before lunge
        [SerializeField] float chargeDuration  = 0.6f;   // seconds of actual charge
        [SerializeField] float slamRadius      = 2.2f;
        [SerializeField] float slamDamage      = 15f;

        bool  isWindingUp;
        bool  isCharging;
        float windupTimer;
        float chargeTimer;
        Vector2 chargeDir;

        protected override void Awake()
        {
            // Override defaults before base.Awake
            maxHealth    = 200f;
            scoreValue   = 100;
            attackDamage = 30f;
            attackCooldown = 3f;
            placeholderColor = new Color(0.35f, 0.15f, 0.65f); // deep purple
            base.Awake();

            // Make the whale visually larger
            transform.localScale = new Vector3(2f, 2f, 1f);
        }

        // ── Chase: slow trudge toward player ─────────────────────────────

        protected override void UpdateChase()
        {
            if (playerTransform == null) return;

            float dist = DistanceToPlayer();

            if (dist <= chargeRange && attackTimer <= 0f)
            {
                StartWindup();
                return;
            }

            Vector2 dir = DirectionToPlayer();
            rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

            if (spriteRenderer != null && dir.x != 0f)
                spriteRenderer.flipX = dir.x < 0f;
        }

        // ── Attack: windup → charge ───────────────────────────────────────

        protected override void UpdateAttack()
        {
            if (isWindingUp)
            {
                windupTimer -= Time.deltaTime;
                rb.linearVelocity = Vector2.zero;

                if (windupTimer <= 0f)
                {
                    isWindingUp = false;
                    isCharging  = true;
                    chargeDir   = DirectionToPlayer();
                    chargeTimer = chargeDuration;
                }
            }
            else if (isCharging)
            {
                chargeTimer -= Time.deltaTime;
                rb.linearVelocity = new Vector2(chargeDir.x * chargeSpeed, rb.linearVelocity.y);

                if (chargeTimer <= 0f)
                    EndCharge();
            }
        }

        void StartWindup()
        {
            SetState(EnemyState.Attack);
            isWindingUp = true;
            windupTimer = chargeWindup;
            isCharging  = false;
        }

        void EndCharge()
        {
            isCharging        = false;
            rb.linearVelocity = Vector2.zero;
            attackTimer       = attackCooldown;

            // Slam AOE around landing point
            var hits = Physics2D.OverlapCircleAll(transform.position, slamRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                    hit.GetComponent<PlayerStats>()?.TakeDamage(slamDamage);
            }

            SetState(EnemyState.Chase);
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (isCharging && col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<PlayerStats>()?.TakeDamage(attackDamage);
                EndCharge();
            }
        }

        // Draw slam radius gizmo in editor
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, slamRadius);
            Gizmos.color = new Color(0.3f, 0.5f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, chargeRange);
        }
    }
}
