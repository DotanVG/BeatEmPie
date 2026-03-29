using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Pumpkin Pie — Ultimate AOE. Giant blast hits ALL enemies on screen.
    /// PLACEHOLDER: large orange circle. ARTIST: needs screen-filling explosion animation.
    /// SFX: massive boom + screen shake. Rare cooldown (8s).
    /// </summary>
    public class PumpkinPie : PieBase
    {
        [Header("Pumpkin — Ultimate")]
        [SerializeField] float blastRadius     = 20f;   // covers entire screen
        [SerializeField] float blastDamage     = 60f;
        [SerializeField] float cameraShakeMag  = 0.4f;
        [SerializeField] float cameraShakeDur  = 0.5f;

        protected override void Awake()
        {
            placeholderColor = new Color(0.95f, 0.5f, 0.05f); // pumpkin orange
            pieType          = PieType.Pumpkin;
            damage           = 40f;
            speed            = 8f;
            base.Awake();
        }

        protected override void OnImpact(EnemyBase enemy)
        {
            TriggerBlast();
        }

        protected override void OnImpactGround()
        {
            TriggerBlast();
        }

        void TriggerBlast()
        {
            SplashDamage(blastRadius, blastDamage, StatusEffect.Stunned, 1.5f);
            StartCoroutine(CameraShake());
        }

        IEnumerator CameraShake()
        {
            var cam = Camera.main;
            if (cam == null) yield break;
            var origin  = cam.transform.position;
            float elapsed = 0f;
            while (elapsed < cameraShakeDur)
            {
                elapsed += Time.deltaTime;
                float t  = 1f - elapsed / cameraShakeDur;
                cam.transform.position = origin + (Vector3)Random.insideUnitCircle * cameraShakeMag * t;
                yield return null;
            }
            cam.transform.position = origin;
        }
    }
}
