using UnityEngine;

/// <summary>
/// Base class for all enemies (Fish, Whale, etc.)
/// Handles health, state machine, and taking damage from pies.
/// </summary>
public class EnemyBase : MonoBehaviour
{
    // float maxHealth
    // float currentHealth
    // EnemyState state (Idle, Chase, Attack, Stunned, Dead)

    // TakeDamage(float amount): reduce health, check death
    // Die(): play death anim, drop loot, destroy
    // ApplyStatusEffect(StatusEffect effect): freeze, confuse, burn, etc.
}
