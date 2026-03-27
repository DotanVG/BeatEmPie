using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks which pies Shushki has unlocked and manages selection/cooldowns.
/// </summary>
public class PieInventory : MonoBehaviour
{
    // List of unlocked PieTypes
    // Dictionary<PieType, float> cooldownTimers
    // int currentPieIndex

    // GetCurrentPie(): return active PieType
    // CycleNext() / CyclePrev(): switch selected pie
    // IsOnCooldown(PieType): check cooldown state
    // TriggerCooldown(PieType): start cooldown after throw
}
