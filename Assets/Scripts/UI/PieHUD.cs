using UnityEngine;
using UnityEngine.UI;

namespace BeatEmPie
{
    /// <summary>
    /// Displays current pie selection + cooldown.
    /// Uses OnGUI for the prototype so no Canvas wiring is required.
    /// ARTIST: replace with proper icon sprites + radial cooldown overlay.
    /// When art is ready, disable OnGUI and use the proper UI panel references below.
    /// </summary>
    public class PieHUD : MonoBehaviour
    {
        [Header("Optional Proper UI References (assign when art is ready)")]
        [SerializeField] Text  pieNameText;
        [SerializeField] Image cooldownOverlay;     // Image with FillMethod = Radial360

        PieInventory inventory;

        static readonly string[] PieNames =
        {
            "Apple Pie",        // 0
            "Cherry Pie",       // 1
            "Blueberry Pie",    // 2
            "Lemon Meringue",   // 3
            "Strawberry Pie",   // 4
            "Meat Pie",         // 5
            "Mushroom Pie",     // 6
            "Pumpkin Pie",      // 7
            "Chocolate Pie",    // 8
            "Chili Pie",        // 9
        };

        // ── Lifecycle ─────────────────────────────────────────────────────

        void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) inventory = playerGO.GetComponent<PieInventory>();

            if (inventory != null)
                inventory.OnPieChanged += RefreshProperUI;

            RefreshProperUI();
        }

        void OnDestroy()
        {
            if (inventory != null)
                inventory.OnPieChanged -= RefreshProperUI;
        }

        // ── Proper UI ─────────────────────────────────────────────────────

        void RefreshProperUI()
        {
            if (inventory == null) return;
            var type = inventory.GetCurrentPie();
            if (pieNameText != null)
                pieNameText.text = PieNames[(int)type];
        }

        void Update()
        {
            if (inventory == null || cooldownOverlay == null) return;
            var type     = inventory.GetCurrentPie();
            float rem    = inventory.GetCooldownRemaining(type);
            float dur    = inventory.GetCooldownDuration(type);
            cooldownOverlay.fillAmount = dur > 0f ? rem / dur : 0f;
        }

        // ── Prototype OnGUI overlay ───────────────────────────────────────
        // DESIGNER NOTE: remove this whole method once proper UI art is wired up.

        void OnGUI()
        {
            if (inventory == null) return;

            var type     = inventory.GetCurrentPie();
            float rem    = inventory.GetCooldownRemaining(type);
            float dur    = inventory.GetCooldownDuration(type);
            bool  ready  = rem <= 0f;

            // Bottom-center of screen
            float w = 220f, h = 50f;
            float x = (Screen.width - w) * 0.5f;
            float y = Screen.height - h - 20f;

            var box = new GUIStyle(GUI.skin.box)
            {
                fontSize  = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            box.normal.textColor = ready ? Color.white : new Color(0.8f, 0.8f, 0.8f, 0.7f);

            string pieName = PieNames[(int)type];
            string label   = ready ? $"{pieName}  [SPACE]" : $"{pieName}  {rem:F1}s";
            GUI.Box(new Rect(x, y, w, h), label, box);

            // Cooldown bar
            if (!ready)
            {
                float barW = w * (1f - rem / dur);
                GUI.DrawTexture(new Rect(x, y + h - 5f, barW, 5f), Texture2D.whiteTexture);
            }

            // Pie list (unlocked) above the selected pie
            var unlocked = inventory.GetUnlockedPies();
            for (int i = 0; i < unlocked.Count; i++)
            {
                bool  selected  = i == inventory.GetCurrentIndex();
                float ix        = x + (i - unlocked.Count * 0.5f + 0.5f) * 28f + w * 0.5f;
                float iy        = y - 30f;
                var   dotStyle  = new GUIStyle(GUI.skin.label) { fontSize = selected ? 18 : 12, alignment = TextAnchor.MiddleCenter };
                dotStyle.normal.textColor = selected ? Color.yellow : Color.gray;
                GUI.Label(new Rect(ix - 14f, iy, 28f, 24f), selected ? "●" : "○", dotStyle);
            }
        }
    }
}
