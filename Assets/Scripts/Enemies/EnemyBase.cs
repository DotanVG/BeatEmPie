using System;
using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    public enum EnemyState { Idle, Chase, Attack, Stunned, Dead }

    /// <summary>
    /// Abstract base class for all enemies.
    /// Provides: health, state machine, damage flash, death, score reporting.
    /// PLACEHOLDER: visuals are solid-color sprites. Replace with spritesheets per ARTIST_SPEC.md.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(StatusEffectHandler))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth     = 50f;
        [SerializeField] protected int   scoreValue    = 10;
        [SerializeField] protected float attackDamage  = 10f;
        [SerializeField] protected float attackCooldown = 1f;

        [Header("Placeholder Art — REPLACE WITH SPRITESHEET")]
        [SerializeField] protected Color placeholderColor = Color.red;

        protected float               currentHealth;
        protected EnemyState          state = EnemyState.Idle;
        protected SpriteRenderer      spriteRenderer;
        protected StatusEffectHandler statusEffects;
        protected Rigidbody2D         rb;
        protected Transform           playerTransform;
        protected float               attackTimer;

        public bool  IsDead        => state == EnemyState.Dead;
        public float CurrentHealth => currentHealth;
        public float MaxHealth     => maxHealth;

        public event Action               OnDeathEvent;
        public event Action<float, float> OnHealthChanged;

        // ── Lifecycle ─────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            rb             = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            statusEffects  = GetComponent<StatusEffectHandler>();
            currentHealth  = maxHealth;
            rb.freezeRotation = true;

            if (spriteRenderer != null)
                spriteRenderer.color = placeholderColor;
        }

        protected virtual void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            SetState(EnemyState.Chase);
        }

        protected virtual void Update()
        {
            if (IsDead) return;
            if (attackTimer > 0f) attackTimer -= Time.deltaTime;

            // Don't act while frozen or stunned
            if (statusEffects != null &&
               (statusEffects.HasEffect(StatusEffect.Frozen) ||
                statusEffects.HasEffect(StatusEffect.Stunned))) return;

            UpdateStateMachine();
        }

        // ── State machine ─────────────────────────────────────────────────

        protected virtual void UpdateStateMachine()
        {
            switch (state)
            {
                case EnemyState.Chase:  UpdateChase();  break;
                case EnemyState.Attack: UpdateAttack(); break;
                case EnemyState.Idle:   UpdateIdle();   break;
            }
        }

        protected virtual void UpdateIdle()   { }
        protected virtual void UpdateChase()  { }
        protected virtual void UpdateAttack() { }

        protected void SetState(EnemyState next)
        {
            if (state == next || state == EnemyState.Dead) return;
            state = next;
        }

        // ── Damage & Death ────────────────────────────────────────────────

        public virtual void TakeDamage(float amount)
        {
            if (IsDead) return;
            currentHealth = Mathf.Max(0f, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            StartCoroutine(DamageFlash());
            if (currentHealth <= 0f) Die();
        }

        public virtual void ApplyStatusEffect(StatusEffect effect, float duration)
        {
            statusEffects?.ApplyEffect(effect, duration);
        }

        protected virtual void Die()
        {
            SetState(EnemyState.Dead);
            rb.linearVelocity = Vector2.zero;
            rb.constraints    = RigidbodyConstraints2D.FreezeAll;
            OnDeathEvent?.Invoke();
            GameManager.Instance?.AddScore(scoreValue);
            EnemySpawner.Instance?.NotifyEnemyDied(gameObject);
            Destroy(gameObject, 0.4f);
        }

        // ── Helpers ───────────────────────────────────────────────────────

        protected float DistanceToPlayer() =>
            playerTransform == null ? float.MaxValue
            : Vector2.Distance(transform.position, playerTransform.position);

        protected Vector2 DirectionToPlayer() =>
            playerTransform == null ? Vector2.zero
            : ((Vector2)(playerTransform.position - transform.position)).normalized;

        IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;
            var orig = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.08f);
            if (spriteRenderer != null) spriteRenderer.color = orig;
        }
    }
}
