using UnityEngine;
using UnityEngine.InputSystem;

namespace BeatEmPie
{
    /// <summary>
    /// Handles player attack input: reads current pie from PieInventory,
    /// checks cooldown, spawns pie prefab and launches it toward nearest enemy.
    /// WIRING: assign piePrefabs[0..9] in Inspector matching PieType enum order.
    /// throwOrigin: child transform positioned at the player's throwing hand.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(PieInventory))]
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Pie Throwing")]
        [SerializeField] Transform throwOrigin;

        [Header("Pie Prefabs — index matches PieType enum (0=Apple … 9=Chili)")]
        [SerializeField] GameObject[] piePrefabs = new GameObject[10];

        [Header("Throw Settings")]
        [SerializeField] float throwAngle = -35f;   // degrees below horizontal
        [SerializeField] float throwRange = 15f;    // max distance to auto-aim

        PlayerStats  stats;
        PieInventory inventory;
        SpriteRenderer sr;

        void Awake()
        {
            stats     = GetComponent<PlayerStats>();
            inventory = GetComponent<PieInventory>();
            sr        = GetComponent<SpriteRenderer>();
        }

        // ── Input callbacks (Unity Input System) ─────────────────────────

        public void OnAttack(InputValue value)
        {
            if (!value.isPressed) return;
            if (stats.IsDead) return;
            if (GameManager.Instance?.State != GameState.Playing) return;

            var pieType = inventory.SelectedPie;
            if (inventory.IsOnCooldown(pieType)) return;
            if (!inventory.HasAmmo(pieType)) return;   // no pies of this type

            ThrowPie(pieType);
        }

        // Allow number keys 1-9, 0 to select hotbar slots directly
        void Update()
        {
            for (int i = 0; i <= 9; i++)
            {
                var key = i == 0 ? KeyCode.Alpha0 : (KeyCode)(KeyCode.Alpha1 + i - 1);
                if (Input.GetKeyDown(key))
                {
                    inventory?.SelectSlot(i == 0 ? 9 : i - 1);
                    break;
                }
            }
        }

        public void OnSwitchPieNext(InputValue value)
        {
            if (!value.isPressed) return;
            inventory.CycleNext();
        }

        public void OnSwitchPiePrev(InputValue value)
        {
            if (!value.isPressed) return;
            inventory.CyclePrev();
        }

        // ── Bootstrap API ────────────────────────────────────────────────

        /// <summary>Called by GameBootstrapper if Inspector fields are null.</summary>
        public void Configure(GameObject[] pies, Transform origin)
        {
            if (piePrefabs == null || piePrefabs.Length == 0 || piePrefabs[0] == null)
                piePrefabs = pies;
            if (throwOrigin == null)
                throwOrigin = origin;
        }

        // ── Throwing ──────────────────────────────────────────────────────

        void ThrowPie(PieType type)
        {
            int idx = (int)type;
            if (idx >= piePrefabs.Length || piePrefabs[idx] == null)
            {
                Debug.LogWarning($"[PlayerCombat] No prefab assigned for pie type {type} (index {idx})");
                return;
            }

            Vector3 origin = throwOrigin != null ? throwOrigin.position : transform.position + Vector3.up * 0.5f;
            Vector2 dir    = GetThrowDirection();

            var pieGO = Instantiate(piePrefabs[idx], origin, Quaternion.identity);
            pieGO.SetActive(true);
            pieGO.GetComponent<PieBase>()?.Launch(dir);

            inventory.ConsumePie(type);   // decrements quantity + starts cooldown
        }

        Vector2 GetThrowDirection()
        {
            // Prefer aiming at nearest enemy in range
            var enemy = FindNearestEnemy();
            if (enemy != null)
            {
                Vector2 toEnemy = (enemy.transform.position - transform.position);
                return toEnemy.normalized;
            }

            // Fallback: throw in the direction the player faces with slight downward arc
            float facingX = (sr != null && sr.flipX) ? -1f : 1f;
            float rad     = throwAngle * Mathf.Deg2Rad;
            return new Vector2(facingX * Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }

        Transform FindNearestEnemy()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform nearest  = null;
            float     bestDist = float.MaxValue;
            foreach (var e in enemies)
            {
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < bestDist && d < throwRange) { bestDist = d; nearest = e.transform; }
            }
            return nearest;
        }
    }
}
