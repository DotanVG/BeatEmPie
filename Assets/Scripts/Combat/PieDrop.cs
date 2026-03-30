using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// A pie collectible that spawns in the world and can be picked up by the player.
    /// When the player walks over it, the pie type + quantity are added to PieInventory.
    ///
    /// PLACEHOLDER: colored circle with quantity label via OnGUI.
    /// ARTIST: replace with animated floating pie sprite + sparkle trail.
    /// SFX: happy pickup jingle (per pie type variant).
    ///
    /// SPAWNING: called from EnemyBase.Die() via PieDropTable.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieDrop : MonoBehaviour
    {
        public PieType PieType { get; private set; }
        public int     Quantity { get; private set; }

        SpriteRenderer sr;
        float bobOffset;

        static readonly Color[] PieColors =
        {
            new Color(0.6f,  0.9f,  0.2f),   // Apple
            new Color(0.9f,  0.1f,  0.15f),  // Cherry
            new Color(0.3f,  0.35f, 0.95f),  // Blueberry
            new Color(1f,    0.95f, 0.1f),   // LemonMeringue
            new Color(1f,    0.4f,  0.6f),   // Strawberry
            new Color(0.55f, 0.25f, 0.1f),   // Meat
            new Color(0.6f,  0.2f,  0.8f),   // Mushroom
            new Color(0.95f, 0.5f,  0.05f),  // Pumpkin
            new Color(0.35f, 0.18f, 0.05f),  // Chocolate
            new Color(1f,    0.3f,  0.0f),   // Chili
        };

        // ── Initialization ────────────────────────────────────────────────

        public void Init(PieType type, int qty)
        {
            PieType  = type;
            Quantity = qty;

            sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = PieColors[(int)type];

            transform.localScale = Vector3.one * 0.4f;

            // Gentle bounce on spawn
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 1.5f;
                rb.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(3f, 6f)), ForceMode2D.Impulse);
            }

            var col       = GetComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 1f;

            // Auto-destroy after 20 seconds if not picked up
            Destroy(gameObject, 20f);
            StartCoroutine(BobAnimation());
        }

        // ── Pickup ────────────────────────────────────────────────────────

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            var inventory = other.GetComponent<PieInventory>();
            if (inventory == null) return;

            inventory.AddPies(PieType, Quantity);
            Destroy(gameObject);
        }

        // ── Placeholder visual ────────────────────────────────────────────

        IEnumerator BobAnimation()
        {
            float startY = transform.position.y;
            // Wait for physics to settle
            yield return new WaitForSeconds(0.8f);

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
            }

            float t = Random.Range(0f, Mathf.PI * 2f);
            while (gameObject != null)
            {
                t += Time.deltaTime * 2f;
                var pos = transform.position;
                pos.y = startY + Mathf.Sin(t) * 0.15f;
                transform.position = pos;
                yield return null;
            }
        }

        // Show quantity + pie type label above the drop
        void OnGUI()
        {
            if (Camera.main == null) return;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.z < 0) return;

            float x = screenPos.x - 20f;
            float y = Screen.height - screenPos.y - 40f;

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.white;

            string label = Quantity == PieInventory.Unlimited ? "∞" : Quantity.ToString();
            GUI.Label(new Rect(x, y, 40f, 20f), label, style);
        }
    }
}
