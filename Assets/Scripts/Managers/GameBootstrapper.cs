using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BeatEmPie
{
    /// <summary>
    /// Auto-configures the entire scene at runtime if Inspector references are not assigned.
    /// Place this component on the GameManager or any persistent scene object.
    ///
    /// What it creates automatically:
    ///  • FishEnemy and WhaleEnemy template GameObjects (used as Instantiate source)
    ///  • 10 pie template GameObjects (one per PieType)
    ///  • 3 off-screen spawn points (left, right, far-right)
    ///  • UI Canvas with HealthBar, PieHUD, GameUI
    ///  • ThrowOrigin child on the player
    ///  • PieInventory component on the player (if missing)
    ///
    /// Placeholder sprites are solid-colored squares generated from Texture2D.
    /// ARTIST: replace with actual sprites — see ARTIST_SPEC.md.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        // Spawn point positions relative to scene center (x, y)
        static readonly Vector2[] SpawnOffsets =
        {
            new Vector2(-12f, 0f),   // far left
            new Vector2( 12f, 0f),   // far right
            new Vector2( 18f, 0f),   // very far right (whale entrance)
        };

        void Awake()
        {
            // --- Find Player ---
            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO == null)
            {
                Debug.LogWarning("[Bootstrap] No GameObject tagged 'Player' found. Tag the player prefab as 'Player'.");
                return;
            }

            EnsurePieInventory(playerGO);
            EnsureThrowOrigin(playerGO, out Transform throwOrigin);

            // --- Placeholder Sprites Setup ---
            // Enemy sprites
            var fishSprite  = MakeSquareSprite(new Color(0.2f, 0.55f, 1f),   80, 50, "FishSprite");
            var whaleSprite = MakeSquareSprite(new Color(0.35f, 0.15f, 0.65f), 120, 80, "WhaleSprite");

            // Pie sprites (one per PieType)
            Color[] pieColors =
            {
                new Color(0.6f,  0.9f,  0.2f),   // Apple — green
                new Color(0.9f,  0.1f,  0.15f),  // Cherry — red
                new Color(0.3f,  0.35f, 0.95f),  // Blueberry — blue
                new Color(1f,    0.95f, 0.1f),   // LemonMeringue — yellow
                new Color(1f,    0.4f,  0.6f),   // Strawberry — pink
                new Color(0.55f, 0.25f, 0.1f),   // Meat — brown
                new Color(0.6f,  0.2f,  0.8f),   // Mushroom — purple
                new Color(0.95f, 0.5f,  0.05f),  // Pumpkin — orange
                new Color(0.35f, 0.18f, 0.05f),  // Chocolate — dark brown
                new Color(1f,    0.3f,  0.0f),   // Chili — orange-red
            };

            // --- Enemy Templates ---
            var fishTemplate  = BuildEnemyTemplate("FishEnemy_Template",  typeof(FishEnemy),  fishSprite,  new Vector2(0.8f, 0.5f));
            var whaleTemplate = BuildEnemyTemplate("WhaleEnemy_Template", typeof(WhaleEnemy), whaleSprite, new Vector2(1.2f, 0.8f), scale: 2f);

            // --- Pie Templates ---
            var piePrefabs = BuildPiePrefabs(pieColors);

            // --- Spawn Points ---
            var spawnPoints = EnsureSpawnPoints();

            // --- Wire EnemySpawner ---
            var spawner = EnemySpawner.Instance;
            if (spawner != null)
                spawner.Configure(fishTemplate, whaleTemplate, spawnPoints);
            else
                Debug.LogWarning("[Bootstrap] EnemySpawner not found in scene.");

            // --- Wire PlayerCombat ---
            var combat = playerGO.GetComponent<PlayerCombat>();
            if (combat != null)
                combat.Configure(piePrefabs, throwOrigin);
            else
                Debug.LogWarning("[Bootstrap] PlayerCombat not found on Player.");

            // --- UI Canvas ---
            EnsureUICanvas(playerGO);

            // --- Ground tag ---
            EnsureGroundTag();

            Debug.Log("[Bootstrap] Scene auto-configured. Hit Play!");
        }

        // ── Player setup ──────────────────────────────────────────────────

        void EnsurePieInventory(GameObject playerGO)
        {
            if (playerGO.GetComponent<PieInventory>() == null)
                playerGO.AddComponent<PieInventory>();
        }

        void EnsureThrowOrigin(GameObject playerGO, out Transform throwOrigin)
        {
            var existing = playerGO.transform.Find("ThrowOrigin");
            if (existing != null)
            {
                throwOrigin = existing;
                return;
            }
            var go = new GameObject("ThrowOrigin");
            go.transform.SetParent(playerGO.transform);
            go.transform.localPosition = new Vector3(0.4f, 0.6f, 0f);
            throwOrigin = go.transform;
        }

        // ── Enemy templates ───────────────────────────────────────────────

        GameObject BuildEnemyTemplate(string name, System.Type enemyScript, Sprite sprite,
                                       Vector2 colliderSize, float scale = 1f)
        {
            var go = new GameObject(name);
            go.SetActive(false); // keep deactivated so it acts as a prefab template
            go.tag = "Enemy";
            go.transform.localScale = Vector3.one * scale;

            var sr     = go.AddComponent<SpriteRenderer>();
            sr.sprite  = sprite;

            var col    = go.AddComponent<BoxCollider2D>();
            col.size   = colliderSize;

            var rb          = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
            rb.freezeRotation = true;
            rb.constraints    = RigidbodyConstraints2D.FreezeRotation;

            go.AddComponent<StatusEffectHandler>();
            go.AddComponent(enemyScript);

            DontDestroyOnLoad(go);
            return go;
        }

        // ── Pie templates ─────────────────────────────────────────────────

        static readonly System.Type[] PieScripts =
        {
            typeof(ApplePie),
            typeof(CherryPie),
            typeof(BlueberryPie),
            typeof(LemonMeringuePie),
            typeof(StrawberryPie),
            typeof(MeatPie),
            typeof(MushroomPie),
            typeof(PumpkinPie),
            typeof(ChocolatePie),
            typeof(ChiliPie),
        };

        GameObject[] BuildPiePrefabs(Color[] colors)
        {
            var prefabs = new GameObject[10];
            for (int i = 0; i < 10; i++)
            {
                var sprite = MakeCircleSprite(colors[i], 32, $"PieSprite_{i}");
                var go     = new GameObject($"Pie_{(PieType)i}_Template");
                go.SetActive(false);
                go.tag = "Projectile";

                var sr    = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.color  = colors[i];
                go.transform.localScale = Vector3.one * 0.35f;

                var rb         = go.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0.3f;
                rb.freezeRotation = false;

                var col       = go.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius    = 1f;

                go.AddComponent(PieScripts[i]);

                DontDestroyOnLoad(go);
                prefabs[i] = go;
            }
            return prefabs;
        }

        // ── Spawn points ──────────────────────────────────────────────────

        Transform[] EnsureSpawnPoints()
        {
            var points = new List<Transform>();

            // Reuse any existing spawn points in scene by name
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.name.ToLower().Contains("spawnpoint") || go.name.ToLower().Contains("spawn_point"))
                    points.Add(go.transform);
            }

            if (points.Count == 0)
            {
                foreach (var offset in SpawnOffsets)
                {
                    var sp = new GameObject("SpawnPoint");
                    sp.transform.position = new Vector3(offset.x, offset.y, 0f);
                    points.Add(sp.transform);
                }
            }

            return points.ToArray();
        }

        // ── UI Canvas ─────────────────────────────────────────────────────

        void EnsureUICanvas(GameObject playerGO)
        {
            // Don't create a second canvas if one already exists
            if (Object.FindAnyObjectByType<Canvas>() != null) return;

            var canvasGO = new GameObject("UICanvas");
            var canvas   = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight  = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // Health Bar (Slider)
            BuildHealthBar(canvasGO.transform, playerGO);

            // Pie HUD
            canvasGO.AddComponent<PieHUD>();   // OnGUI driven

            // Game UI overlay
            canvasGO.AddComponent<GameUI>();
        }

        void BuildHealthBar(Transform canvasTransform, GameObject playerGO)
        {
            // Container
            var barGO  = new GameObject("HealthBar");
            barGO.transform.SetParent(canvasTransform);
            var rect   = barGO.AddComponent<RectTransform>();
            rect.anchorMin   = new Vector2(0f, 1f);
            rect.anchorMax   = new Vector2(0f, 1f);
            rect.pivot       = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(20f, -20f);
            rect.sizeDelta   = new Vector2(300f, 30f);

            // Background
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(barGO.transform);
            var bgRect = bgGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero; bgRect.anchoredPosition = Vector2.zero;
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.05f, 0.05f, 0.85f);

            // Fill area
            var fillAreaGO = new GameObject("Fill Area");
            fillAreaGO.transform.SetParent(barGO.transform);
            var faRect = fillAreaGO.AddComponent<RectTransform>();
            faRect.anchorMin = Vector2.zero; faRect.anchorMax = Vector2.one;
            faRect.sizeDelta = new Vector2(-10f, -6f); faRect.anchoredPosition = Vector2.zero;

            // Fill
            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillAreaGO.transform);
            var fillRect = fillGO.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero; fillRect.anchoredPosition = Vector2.zero;
            var fillImg = fillGO.AddComponent<Image>();
            fillImg.color = new Color(0.85f, 0.15f, 0.15f);

            // Slider
            var slider = barGO.AddComponent<Slider>();
            slider.fillRect    = fillRect;
            slider.minValue    = 0f;
            slider.maxValue    = 100f;
            slider.value       = 100f;
            slider.interactable = false;
            slider.transition  = Selectable.Transition.None;

            // HealthBar component
            var hb = barGO.AddComponent<HealthBar>();
            _ = hb;   // auto-finds PlayerStats
        }

        // ── Ground tag ────────────────────────────────────────────────────

        void EnsureGroundTag()
        {
            // Try to find a ground object by common names
            string[] groundNames = { "Ground", "Platform", "Floor", "Tilemap", "TilemapCollider" };
            foreach (var n in groundNames)
            {
                var go = GameObject.Find(n);
                if (go != null && go.GetComponent<Collider2D>() != null)
                {
                    go.tag = "Ground";
                    return;
                }
            }
            // Also tag anything on the "Ground" layer
            var all = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
            foreach (var col in all)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    col.gameObject.tag = "Ground";
            }
        }

        // ── Sprite generation ─────────────────────────────────────────────

        static Sprite MakeSquareSprite(Color color, int w, int h, string name)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.name = name;
            var pixels = new Color[w * h];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), Mathf.Min(w, h));
        }

        static Sprite MakeCircleSprite(Color color, int size, string name)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.name = name;
            float cx = size * 0.5f - 0.5f, cy = size * 0.5f - 0.5f, r = size * 0.5f - 1f;
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx, dy = y - cy;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    pixels[y * size + x] = dist <= r ? color : Color.clear;
                }
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
