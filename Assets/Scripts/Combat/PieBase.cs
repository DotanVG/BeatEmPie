using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Abstract base for all pie projectiles.
    /// Launched via Launch(Vector2 dir). Destroys on enemy hit or ground.
    /// PLACEHOLDER: solid colored circle. Replace with animated spritesheet per ARTIST_SPEC.md.
    /// Each pie type needs: Fly loop, Spin, Impact burst animations.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class PieBase : MonoBehaviour
    {
        [Header("Pie Stats")]
        [SerializeField] protected float damage   = 20f;
        [SerializeField] protected float speed    = 12f;
        [SerializeField] protected float lifetime = 4f;

        [Header("Placeholder Art — REPLACE WITH PIE SPRITE")]
        [SerializeField] protected Color placeholderColor = Color.yellow;

        protected PieType        pieType;
        protected Rigidbody2D    rb;
        protected SpriteRenderer sr;
        bool                     hasHit;

        // ── Lifecycle ─────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            rb.gravityScale   = 0.3f;   // slight arc
            rb.freezeRotation = false;

            // Trigger so pies pass-through on first hit
            var col = GetComponent<CircleCollider2D>();
            col.isTrigger = true;

            if (sr != null) sr.color = placeholderColor;
        }

        protected virtual void Start()
        {
            Destroy(gameObject, lifetime);
        }

        void Update()
        {
            // Spin the pie sprite while flying
            transform.Rotate(0f, 0f, 360f * Time.deltaTime);
        }

        // ── Launch ────────────────────────────────────────────────────────

        /// <summary>Called by PlayerCombat to give the pie its initial velocity.</summary>
        public void Launch(Vector2 direction)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        // ── Collision ─────────────────────────────────────────────────────

        void OnTriggerEnter2D(Collider2D other)
        {
            if (hasHit) return;

            if (other.CompareTag("Enemy") && other.TryGetComponent<EnemyBase>(out var enemy))
            {
                hasHit = true;
                enemy.TakeDamage(damage);
                OnImpact(enemy);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Ground"))
            {
                hasHit = true;
                OnImpactGround();
                Destroy(gameObject);
            }
        }

        // ── Overrides ─────────────────────────────────────────────────────

        /// <summary>Called when this pie hits an enemy. Override for special effects.</summary>
        protected virtual void OnImpact(EnemyBase enemy) { }

        /// <summary>Called when this pie hits the ground. Override for ground effects.</summary>
        protected virtual void OnImpactGround() { }

        // ── Helper ────────────────────────────────────────────────────────

        /// <summary>Apply a status effect to all enemies within radius of current position.</summary>
        protected void SplashDamage(float radius, float dmg, StatusEffect effect = StatusEffect.None, float effectDuration = 0f)
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                if (!hit.TryGetComponent<EnemyBase>(out var e)) continue;
                e.TakeDamage(dmg);
                if (effect != StatusEffect.None)
                    e.ApplyStatusEffect(effect, effectDuration);
            }
        }
    }
}
