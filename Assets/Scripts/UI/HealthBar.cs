using UnityEngine;
using UnityEngine.UI;

namespace BeatEmPie
{
    /// <summary>
    /// Drives the health bar Slider from PlayerStats events.
    /// Auto-finds PlayerStats if not assigned. Smooth lerp animation.
    /// PLACEHOLDER: Unity default Slider. ARTIST: replace with custom health bar sprite.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Slider      slider;
        [SerializeField] PlayerStats playerStats;   // auto-found if null

        [Header("Animation")]
        [SerializeField] float lerpSpeed = 8f;

        float targetValue;

        // ── Lifecycle ─────────────────────────────────────────────────────

        void Start()
        {
            if (playerStats == null)
            {
                var playerGO = GameObject.FindGameObjectWithTag("Player");
                if (playerGO != null) playerStats = playerGO.GetComponent<PlayerStats>();
            }

            if (playerStats != null)
            {
                playerStats.OnHealthChanged += OnHealthChanged;
                SetMax(playerStats.MaxHealth);
                SetTarget(playerStats.CurrentHealth);
                if (slider != null) slider.value = targetValue;
            }
        }

        void OnDestroy()
        {
            if (playerStats != null)
                playerStats.OnHealthChanged -= OnHealthChanged;
        }

        // ── Events ────────────────────────────────────────────────────────

        void OnHealthChanged(float current, float max)
        {
            SetMax(max);
            SetTarget(current);
        }

        // ── Update ────────────────────────────────────────────────────────

        void Update()
        {
            if (slider == null) return;
            slider.value = Mathf.Lerp(slider.value, targetValue, lerpSpeed * Time.deltaTime);
        }

        // ── Helpers ───────────────────────────────────────────────────────

        void SetMax(float max)
        {
            if (slider == null) return;
            slider.minValue = 0f;
            slider.maxValue = max;
        }

        void SetTarget(float current)
        {
            targetValue = current;
        }
    }
}
