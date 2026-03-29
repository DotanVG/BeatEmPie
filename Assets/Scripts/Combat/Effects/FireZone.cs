using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Spawned by ChiliPie. A lingering fire zone that applies Burning to any enemy inside it.
    /// PLACEHOLDER: orange transparent circle. ARTIST: animated flame sprite, looping fire FX.
    /// SFX: crackling fire loop (attach AudioSource on this object).
    /// </summary>
    public class FireZone : MonoBehaviour
    {
        float radius;
        float lifetime;
        float burnDuration;
        float tickInterval = 0.3f;
        float tickTimer;

        SpriteRenderer sr;

        public void Init(float r, float life, float burnDur)
        {
            radius       = r;
            lifetime     = life;
            burnDuration = burnDur;

            // Placeholder: a colored circle scaled to radius
            sr = gameObject.AddComponent<SpriteRenderer>();
            sr.color = new Color(1f, 0.4f, 0f, 0.45f);
            transform.localScale = Vector3.one * radius * 2f;

            // Add a trigger collider
            var col       = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.5f; // world radius = 0.5 * scale = radius

            Destroy(gameObject, lifetime);
            StartCoroutine(FadeOut());
        }

        void Update()
        {
            tickTimer -= Time.deltaTime;
            if (tickTimer > 0f) return;
            tickTimer = tickInterval;

            var hits = Physics2D.OverlapCircleAll(transform.position, radius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;
                hit.GetComponent<EnemyBase>()?.ApplyStatusEffect(StatusEffect.Burning, burnDuration);
            }
        }

        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(lifetime - 0.5f);
            float t = 0f;
            var startAlpha = sr.color.a;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                sr.color = new Color(1f, 0.4f, 0f, Mathf.Lerp(startAlpha, 0f, t / 0.5f));
                yield return null;
            }
        }
    }
}
