using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Manages all active status effects on an enemy.
    /// Handles applying, ticking down, and removing effects.
    /// PLACEHOLDER: visual feedback uses sprite color tinting only.
    /// Replace with particle effects / shader swaps once art is ready.
    /// </summary>
    public class StatusEffectHandler : MonoBehaviour
    {
        readonly Dictionary<StatusEffect, float> activeEffects = new();
        EnemyBase enemyBase;
        Rigidbody2D rb;
        SpriteRenderer spriteRenderer;
        Color baseColor;

        const float BurnDps = 8f;

        public event Action<StatusEffect> OnEffectApplied;
        public event Action<StatusEffect> OnEffectRemoved;

        void Awake()
        {
            enemyBase      = GetComponent<EnemyBase>();
            rb             = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            if (spriteRenderer != null)
                baseColor = spriteRenderer.color;
        }

        // ── Public API ────────────────────────────────────────────────────

        public void ApplyEffect(StatusEffect effect, float duration)
        {
            if (enemyBase != null && enemyBase.IsDead) return;
            if (effect == StatusEffect.None) return;

            bool isNew = !activeEffects.ContainsKey(effect);
            activeEffects[effect] = duration;

            if (isNew)
            {
                OnEffectApplied?.Invoke(effect);
                ApplyBehavior(effect);
                UpdateTint();
            }
        }

        public void RemoveEffect(StatusEffect effect)
        {
            if (!activeEffects.ContainsKey(effect)) return;
            activeEffects.Remove(effect);
            OnEffectRemoved?.Invoke(effect);
            RemoveBehavior(effect);
            UpdateTint();
        }

        public bool HasEffect(StatusEffect effect)    => activeEffects.ContainsKey(effect);
        public float GetDuration(StatusEffect effect) => activeEffects.TryGetValue(effect, out var t) ? t : 0f;

        // ── Update loop ───────────────────────────────────────────────────

        void Update()
        {
            if (activeEffects.Count == 0) return;

            var toRemove = new List<StatusEffect>();
            var keys     = new List<StatusEffect>(activeEffects.Keys);

            foreach (var effect in keys)
            {
                activeEffects[effect] -= Time.deltaTime;

                if (effect == StatusEffect.Burning && enemyBase != null)
                    enemyBase.TakeDamage(BurnDps * Time.deltaTime);

                if (activeEffects[effect] <= 0f)
                    toRemove.Add(effect);
            }

            foreach (var e in toRemove)
                RemoveEffect(e);
        }

        // ── Behavior helpers ──────────────────────────────────────────────

        void ApplyBehavior(StatusEffect effect)
        {
            switch (effect)
            {
                case StatusEffect.Frozen:
                    if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    break;
                case StatusEffect.Stunned:
                    if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                    break;
            }
        }

        void RemoveBehavior(StatusEffect effect)
        {
            switch (effect)
            {
                case StatusEffect.Frozen:
                case StatusEffect.Stunned:
                    if (!HasEffect(StatusEffect.Frozen) && !HasEffect(StatusEffect.Stunned))
                        if (rb != null) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    break;
            }
        }

        // ── Placeholder tint ──────────────────────────────────────────────
        // ARTIST TODO: replace with particle effects / shader overlays

        void UpdateTint()
        {
            if (spriteRenderer == null) return;

            if (HasEffect(StatusEffect.Frozen))
                spriteRenderer.color = new Color(0.4f, 0.8f, 1f);
            else if (HasEffect(StatusEffect.Burning))
                spriteRenderer.color = new Color(1f, 0.4f, 0.1f);
            else if (HasEffect(StatusEffect.Confused))
                spriteRenderer.color = new Color(0.9f, 0.4f, 0.9f);
            else if (HasEffect(StatusEffect.Electrified))
                spriteRenderer.color = new Color(1f, 1f, 0.2f);
            else if (HasEffect(StatusEffect.Slowed))
                spriteRenderer.color = new Color(0.6f, 0.4f, 0.2f);
            else if (HasEffect(StatusEffect.Stunned))
                spriteRenderer.color = new Color(1f, 0.9f, 0.2f);
            else
                spriteRenderer.color = baseColor;
        }
    }
}
