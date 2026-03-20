using UnityEngine;
using System;

/// <summary>
/// Tracks Shushki's runtime stats — health, speed, and damage events.
/// Communicates with HealthBar UI via events.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // float maxHealth
    // float currentHealth

    // float moveSpeed
    // float jumpForce

    // Action OnDeath event — fired when health reaches zero
    // Action<float, float> OnHealthChanged — fired with (current, max) for UI

    // Reference to HealthBar UI component

    // Start: initialize health to max

    // TakeDamage(float amount): reduce health, fire OnHealthChanged, check death

    // Heal(float amount): restore health, clamp to max, fire OnHealthChanged

    // Die(): fire OnDeath event, trigger death sequence
}
