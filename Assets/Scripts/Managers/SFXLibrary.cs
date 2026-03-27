using UnityEngine;

namespace BeatEmPie
{
    [CreateAssetMenu(fileName = "SFXLibrary", menuName = "BeatEmPie/SFX Library")]
    public class SFXLibrary : ScriptableObject
    {
        [Header("Pie — Generic")]
        public AudioClip pieThrow;
        public AudioClip pieImpactGeneric;

        [Header("Pie — Per Type Impacts")]
        public AudioClip pieImpactApple;         // Standard thwack
        public AudioClip pieImpactCherry;        // Explosion boom
        public AudioClip pieImpactBlueberry;     // Freeze whoosh/crack
        public AudioClip pieImpactLemonMeringue; // Electric zap
        public AudioClip pieImpactStrawberry;    // Homing whoosh
        public AudioClip pieImpactMeat;          // Heavy thud
        public AudioClip pieImpactMushroom;      // Cartoony boing
        public AudioClip pieImpactPumpkin;       // Massive boom
        public AudioClip pieImpactChocolate;     // Wet splat
        public AudioClip pieImpactChili;         // Sizzle/fire crackle

        [Header("Enemy")]
        public AudioClip enemyHit;
        public AudioClip enemyDeath;
        public AudioClip whaleStomp;

        [Header("Player")]
        public AudioClip playerHurt;
        public AudioClip playerDeath;
        public AudioClip playerJump;
        public AudioClip playerLand;

        [Header("UI")]
        public AudioClip uiSelect;
        public AudioClip uiConfirm;
        public AudioClip waveStart;
        public AudioClip waveComplete;

        /// <summary>Returns the specific impact clip for a pie type, falling back to generic.</summary>
        public AudioClip GetPieImpact(PieType type)
        {
            return type switch
            {
                PieType.Apple          => pieImpactApple          ? pieImpactApple          : pieImpactGeneric,
                PieType.Cherry         => pieImpactCherry         ? pieImpactCherry         : pieImpactGeneric,
                PieType.Blueberry      => pieImpactBlueberry      ? pieImpactBlueberry      : pieImpactGeneric,
                PieType.LemonMeringue  => pieImpactLemonMeringue  ? pieImpactLemonMeringue  : pieImpactGeneric,
                PieType.Strawberry     => pieImpactStrawberry     ? pieImpactStrawberry     : pieImpactGeneric,
                PieType.Meat           => pieImpactMeat           ? pieImpactMeat           : pieImpactGeneric,
                PieType.Mushroom       => pieImpactMushroom       ? pieImpactMushroom       : pieImpactGeneric,
                PieType.Pumpkin        => pieImpactPumpkin        ? pieImpactPumpkin        : pieImpactGeneric,
                PieType.Chocolate      => pieImpactChocolate      ? pieImpactChocolate      : pieImpactGeneric,
                PieType.Chili          => pieImpactChili          ? pieImpactChili          : pieImpactGeneric,
                _                      => pieImpactGeneric
            };
        }
    }
}
