using UnityEngine;
using System;

namespace BeatEmPie
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] float maxHealth = 100f;

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }

        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;

        void Start()
        {
            CurrentHealth = maxHealth;
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
            if (CurrentHealth <= 0f) Die();
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        void Die()
        {
            IsDead = true;
            OnDeath?.Invoke();
        }
    }
}
