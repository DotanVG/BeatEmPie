using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Strawberry Pie — Homing. Tracks nearest enemy; guarantees a hit.
    /// PLACEHOLDER: pink circle with spiral animation. ARTIST: needs Fly + heart-trail + Splat.
    /// SFX: cute whoosh that curves, sweet splat on impact.
    /// </summary>
    public class StrawberryPie : PieBase
    {
        [Header("Strawberry — Homing")]
        [SerializeField] float homingStrength = 5f;
        [SerializeField] float homingRange    = 12f;

        Transform target;

        protected override void Awake()
        {
            placeholderColor = new Color(1f, 0.4f, 0.6f); // strawberry pink
            pieType          = PieType.Strawberry;
            damage           = 22f;
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            target = FindNearestEnemy();
        }

        // PieBase.Update() handles sprite spin; we add homing on top via FixedUpdate
        void FixedUpdate()
        {
            if (target == null) return;
            if (target.GetComponent<EnemyBase>()?.IsDead == true) { target = FindNearestEnemy(); return; }

            Vector2 desiredDir = ((Vector2)(target.position - transform.position)).normalized;
            Vector2 currentVel = rb.linearVelocity;
            Vector2 newVel     = Vector2.Lerp(currentVel.normalized, desiredDir, homingStrength * Time.fixedDeltaTime) * speed;
            rb.linearVelocity  = newVel;
        }

        Transform FindNearestEnemy()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform nearest = null;
            float     bestDist = float.MaxValue;
            foreach (var e in enemies)
            {
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < bestDist && d < homingRange) { bestDist = d; nearest = e.transform; }
            }
            return nearest;
        }
    }
}
