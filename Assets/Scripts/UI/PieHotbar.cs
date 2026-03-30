using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Minecraft-style pie selection hotbar shown at the bottom-center of the screen.
    ///
    /// Layout:
    ///   [ 1 ] [ 2 ] [ 3 ] [ 4 ] [ 5 ] [ 6 ] [ 7 ] [ 8 ] [ 9 ] [ 0 ]
    ///   ←———————————————————————————————————————————————————————————→
    ///   Apple Cherry Blue Lemon Straw Meat Mush Pump Choc Chili
    ///
    /// Slot states:
    ///   SELECTED  — bright yellow border, enlarged
    ///   READY     — white border, shows quantity bottom-right
    ///   COOLDOWN  — dark overlay + fill bar from bottom
    ///   LOCKED    — grey, shows padlock symbol (not yet unlocked)
    ///   EMPTY     — visible but greyed out (unlocked but qty=0)
    ///
    /// PLACEHOLDER: drawn with OnGUI using colored rectangles + labels.
    /// ARTIST: replace with:
    ///   - Hotbar background sprite (stone texture like MC)
    ///   - Slot frame sprites (normal, selected, locked variants)
    ///   - Pie icon sprites (one per PieType, 64×64 or 128×128)
    ///   - Quantity badge font
    ///   - Cooldown radial or fill overlay shader
    /// </summary>
    public class PieHotbar : MonoBehaviour
    {
        PieInventory inventory;

        const int    SlotCount  = 10;
        const float  SlotSize   = 58f;
        const float  SlotPad    = 4f;
        const float  BarY       = 16f;   // distance from bottom of screen

        static readonly string[] PieNames =
        {
            "Apple",    "Cherry",   "Blue",    "Lemon",   "Straw",
            "Meat",     "Mush",     "Pumpkin", "Choco",   "Chili",
        };

        static readonly Color[] PieColors =
        {
            new Color(0.6f,  0.9f,  0.2f),
            new Color(0.9f,  0.1f,  0.15f),
            new Color(0.3f,  0.35f, 0.95f),
            new Color(1f,    0.95f, 0.1f),
            new Color(1f,    0.4f,  0.6f),
            new Color(0.55f, 0.25f, 0.1f),
            new Color(0.6f,  0.2f,  0.8f),
            new Color(0.95f, 0.5f,  0.05f),
            new Color(0.35f, 0.18f, 0.05f),
            new Color(1f,    0.3f,  0.0f),
        };

        // Cached styles
        GUIStyle slotLabelStyle;
        GUIStyle qtyStyle;
        GUIStyle keyStyle;
        bool stylesInit;

        // ── Lifecycle ─────────────────────────────────────────────────────

        void Start()
        {
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                inventory = playerGO.GetComponent<PieInventory>();

            if (inventory != null)
                inventory.OnInventoryChanged += Repaint;
        }

        void OnDestroy()
        {
            if (inventory != null)
                inventory.OnInventoryChanged -= Repaint;
        }

        void Repaint() { /* triggers next OnGUI pass */ }

        // ── Rendering ─────────────────────────────────────────────────────

        void OnGUI()
        {
            if (inventory == null) return;
            EnsureStyles();

            float totalWidth = SlotCount * SlotSize + (SlotCount - 1) * SlotPad;
            float startX     = (Screen.width - totalWidth) * 0.5f;
            float startY     = Screen.height - SlotSize - BarY;

            // Dark bar background
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(startX - 8f, startY - 6f, totalWidth + 16f, SlotSize + 12f), Texture2D.whiteTexture);
            GUI.color = Color.white;

            for (int i = 0; i < SlotCount; i++)
            {
                var type     = (PieType)i;
                bool selected = i == inventory.SelectedSlot;
                bool unlocked = inventory.IsUnlocked(type);
                int  qty      = inventory.GetQuantity(type);
                bool hasAmmo  = inventory.HasAmmo(type);
                bool onCD     = inventory.IsOnCooldown(type);
                float cdRem   = inventory.GetCooldownRemaining(type);
                float cdDur   = inventory.GetCooldownDuration(type);

                float size = selected ? SlotSize : SlotSize - 4f;
                float ox   = selected ? 0f : 2f;
                float oy   = selected ? 0f : 2f;
                float x    = startX + i * (SlotSize + SlotPad) + ox;
                float y    = startY + oy;
                var   slot = new Rect(x, y, size, size);

                // ── Slot background ──

                // Border color
                Color borderColor = selected ? Color.yellow
                    : unlocked && hasAmmo ? Color.white
                    : unlocked ? new Color(0.8f, 0.5f, 0.5f)     // empty — reddish hint
                    : new Color(0.4f, 0.4f, 0.4f);               // locked — dark grey

                GUI.color = borderColor;
                GUI.DrawTexture(slot, Texture2D.whiteTexture);

                // Inner slot (2px inset)
                var inner = new Rect(slot.x + 2, slot.y + 2, slot.width - 4, slot.height - 4);
                Color bgColor = unlocked ? new Color(PieColors[i].r * 0.3f, PieColors[i].g * 0.3f, PieColors[i].b * 0.3f, 0.95f)
                                         : new Color(0.12f, 0.12f, 0.12f, 0.95f);
                GUI.color = bgColor;
                GUI.DrawTexture(inner, Texture2D.whiteTexture);

                // Pie color swatch (center of slot)
                if (unlocked)
                {
                    float sw  = size * 0.5f;
                    var  swatchRect = new Rect(x + (size - sw) * 0.5f, y + (size - sw) * 0.5f, sw, sw);
                    GUI.color = hasAmmo ? PieColors[i] : new Color(PieColors[i].r, PieColors[i].g, PieColors[i].b, 0.35f);
                    GUI.DrawTexture(swatchRect, Texture2D.whiteTexture);
                }
                GUI.color = Color.white;

                // ── Cooldown overlay ──
                if (onCD && cdDur > 0f)
                {
                    float fillFrac = cdRem / cdDur;
                    var   cdRect   = new Rect(inner.x, inner.y + inner.height * (1f - fillFrac),
                                             inner.width, inner.height * fillFrac);
                    GUI.color = new Color(0f, 0f, 0f, 0.6f);
                    GUI.DrawTexture(cdRect, Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }

                // ── Key hint (top-left) ──
                string keyLabel = i == 9 ? "0" : (i + 1).ToString();
                GUI.Label(new Rect(x + 3f, y + 1f, 18f, 16f), keyLabel, keyStyle);

                // ── Pie name (bottom label) ──
                if (unlocked)
                {
                    GUI.Label(new Rect(x, y + size - 17f, size, 16f),
                              PieNames[i], slotLabelStyle);
                }
                else
                {
                    // Lock icon text
                    GUI.Label(new Rect(x, y + size * 0.25f, size, size * 0.5f), "🔒", slotLabelStyle);
                }

                // ── Quantity badge (bottom-right) ──
                if (unlocked)
                {
                    string qtyLabel = qty == PieInventory.Unlimited ? "∞"
                                    : qty == 0 ? "×"
                                    : qty.ToString();
                    Color qtyColor = qty == 0 ? new Color(1f, 0.4f, 0.4f)
                                   : qty <= 3 ? new Color(1f, 0.85f, 0.2f)
                                   : Color.white;
                    var qs = new GUIStyle(qtyStyle);
                    qs.normal.textColor = qtyColor;
                    GUI.Label(new Rect(x + size - 22f, y + size - 18f, 22f, 18f), qtyLabel, qs);
                }

                // ── Selected glow ──
                if (selected)
                {
                    GUI.color = new Color(1f, 1f, 0f, 0.25f);
                    GUI.DrawTexture(inner, Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }
            }

            // ── Selected pie name tooltip above bar ──
            var selType = inventory.SelectedPie;
            int selQty  = inventory.GetQuantity(selType);
            bool selAmmo = inventory.HasAmmo(selType);
            bool selCD   = inventory.IsOnCooldown(selType);
            float selCdR = inventory.GetCooldownRemaining(selType);

            string tooltip = PieNames[(int)selType] + " Pie";
            if (!inventory.IsUnlocked(selType))
                tooltip += "  [LOCKED]";
            else if (!selAmmo)
                tooltip += "  [EMPTY]";
            else if (selCD)
                tooltip += $"  CD: {selCdR:F1}s";
            else
                tooltip += "  [SPACE]";

            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            float tw = 220f, th = 24f;
            float tx = (Screen.width - tw) * 0.5f;
            float ty = Screen.height - SlotSize - BarY - th - 6f;
            GUI.DrawTexture(new Rect(tx - 6f, ty - 2f, tw + 12f, th + 4f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            var tipStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            tipStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(tx, ty, tw, th), tooltip, tipStyle);
        }

        // ── Style init ────────────────────────────────────────────────────

        void EnsureStyles()
        {
            if (stylesInit) return;
            stylesInit = true;

            slotLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 9,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            slotLabelStyle.normal.textColor = Color.white;

            qtyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleRight,
            };
            qtyStyle.normal.textColor = Color.white;

            keyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 10,
                alignment = TextAnchor.UpperLeft,
            };
            keyStyle.normal.textColor = new Color(1f, 1f, 1f, 0.6f);
        }
    }
}
