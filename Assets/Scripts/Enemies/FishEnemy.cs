using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Fish Enemy — Basic grunt. Chases player, deals contact damage.
    /// PLACEHOLDER: blue rectangle. Replace with Fish spritesheet per ARTIST_SPEC.md.
    /// Animation states needed: Idle, Swim (walk cycle), Hit, Death.
    /// </summary>
    public class FishEnemy : EnemyBase
    {
        [Header("Fish Settings")]
        [SerializeField] float moveSpeed   = 2.8f;
        [SerializeField] float attackRange = 0.9f;

        protected override void Awake()
        {
            // Set fish placeholder color before base.Awake applies it
            placeholderColor = new Color(0.2f, 0.55f, 1f); // ocean blue
            base.Awake();
        }

        // ── Chase: swim toward player ─────────────────────────────────────

        protected override void UpdateChase()
        {
            if (playerTransform == null) return;

            float dist = DistanceToPlayer();
            if (dist <= attackRange)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                SetState(EnemyState.Attack);
                return;
            }

            Vector2 dir = DirectionToPlayer();
            rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

            // Flip sprite to face movement direction
            if (spriteRenderer != null && dir.x != 0f)
                spriteRenderer.flipX = dir.x < 0f;
        }

        // ── Attack: chomp damage ──────────────────────────────────────────

        protected override void UpdateAttack()
        {
            if (attackTimer > 0f) return;

            if (playerTransform != null && DistanceToPlayer() <= attackRange + 0.3f)
            {
                playerTransform.GetComponent<PlayerStats>()?.TakeDamage(attackDamage);
                attackTimer = attackCooldown;
            }
            else
            {
                SetState(EnemyState.Chase);
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player") && attackTimer <= 0f)
            {
                col.gameObject.GetComponent<PlayerStats>()?.TakeDamage(attackDamage);
                attackTimer = attackCooldown;
            }
        }
    }
}
