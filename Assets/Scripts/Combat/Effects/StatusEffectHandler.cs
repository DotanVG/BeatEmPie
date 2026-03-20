using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Component attached to enemies to manage all active status effects.
/// Handles applying, ticking, and removing effects from StatusEffect enum.
/// </summary>
public class StatusEffectHandler : MonoBehaviour
{
    // Dictionary<StatusEffect, float> activeEffects — effect to remaining duration

    // Action<StatusEffect> OnEffectApplied
    // Action<StatusEffect> OnEffectRemoved

    // ApplyEffect(StatusEffect effect, float duration):
    //   add or refresh effect in dictionary, fire OnEffectApplied

    // RemoveEffect(StatusEffect effect):
    //   remove from dictionary, fire OnEffectRemoved, restore normal state

    // HasEffect(StatusEffect effect): bool check

    // Update: tick down all durations, call RemoveEffect on expired ones
}
