using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls Shushki's health bar UI element.
/// Subscribes to PlayerStats events and updates the Slider smoothly.
/// </summary>
public class HealthBar : MonoBehaviour
{
    // Slider slider reference
    // float lerpSpeed — how fast the bar animates

    // Reference to PlayerStats

    // Start: subscribe to OnHealthChanged event, set initial value

    // SetMaxHealth(float max): configure slider max value

    // SetHealth(float current): update slider, trigger lerp

    // Update: lerp slider value toward target for smooth animation
}
