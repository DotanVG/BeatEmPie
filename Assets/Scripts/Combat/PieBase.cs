using UnityEngine;

/// <summary>
/// Base class for all pie projectiles.
/// Each pie type inherits from this and overrides OnImpact().
/// </summary>
public class PieBase : MonoBehaviour
{
    // float damage
    // float speed
    // PieType pieType

    // Move downward / arc toward target

    // OnTriggerEnter2D: detect hit with enemy or ground

    // virtual OnImpact(): called on hit — override in subclasses

    // Destroy self after impact or lifetime expires
}
