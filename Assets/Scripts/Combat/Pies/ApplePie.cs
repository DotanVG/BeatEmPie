using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Apple Pie — Standard attack. Basic damage, no special effect.
    /// Always unlocked. Fast cooldown, reliable.
    /// PLACEHOLDER: yellow-green circle. ARTIST: needs Fly/Spin/Splat animations.
    /// </summary>
    public class ApplePie : PieBase
    {
        protected override void Awake()
        {
            placeholderColor = new Color(0.6f, 0.9f, 0.2f); // apple green
            pieType          = PieType.Apple;
            base.Awake();
        }

        // OnImpact is not overridden — base damage only (already applied by PieBase)
    }
}
