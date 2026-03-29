namespace BeatEmPie
{
    /// <summary>
    /// All possible status effects that can be applied to enemies via pie impacts.
    /// </summary>
    public enum StatusEffect
    {
        None,
        Frozen,       // Blueberry Pie — slows/freezes enemy
        Burning,      // Chili Pie — damage over time from fire trail
        Confused,     // Mushroom Pie — enemy attacks their own allies
        Slowed,       // Chocolate Pie — slipping in chocolate puddle
        Stunned,      // Cherry Pie — brief stun from explosion knockback
        Electrified   // Lemon Meringue — chained lightning effect
    }
}
