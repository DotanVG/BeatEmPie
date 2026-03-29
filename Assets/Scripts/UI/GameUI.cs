using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Prototype UI overlay using OnGUI.
    /// Handles: Main Menu start screen, in-game HUD (score/wave), pause, game over, victory.
    /// DESIGNER: replace OnGUI panels with proper canvas art once assets arrive.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        // Cached styles (built once in Start)
        GUIStyle titleStyle;
        GUIStyle bodyStyle;
        GUIStyle buttonStyle;
        GUIStyle hudStyle;
        bool     stylesInit;

        void OnGUI()
        {
            if (GameManager.Instance == null) return;

            EnsureStyles();

            switch (GameManager.Instance.State)
            {
                case GameState.MainMenu:  DrawMainMenu();  break;
                case GameState.Playing:   DrawHUD();       break;
                case GameState.Paused:    DrawPause();     break;
                case GameState.GameOver:  DrawGameOver();  break;
                case GameState.Victory:   DrawVictory();   break;
            }
        }

        // ── Screens ───────────────────────────────────────────────────────

        void DrawMainMenu()
        {
            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            // Dark overlay
            GUI.color = new Color(0f, 0f, 0f, 0.65f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(cx - 200f, cy - 160f, 400f, 80f), "BEAT-EM PIE", titleStyle);
            GUI.Label(new Rect(cx - 200f, cy - 80f,  400f, 40f), "Shushki rains magical pies upon the fish!", bodyStyle);
            GUI.Label(new Rect(cx - 200f, cy - 40f,  400f, 30f), "WASD / Arrow keys to move  |  Space to throw  |  Q/E to switch pie", bodyStyle);

            if (GUI.Button(new Rect(cx - 100f, cy + 20f, 200f, 50f), "START GAME", buttonStyle))
                GameManager.Instance.StartGame();
        }

        void DrawHUD()
        {
            int score = GameManager.Instance.CurrentScore;
            int wave  = EnemySpawner.Instance != null ? EnemySpawner.Instance.CurrentWave : 0;
            int alive = EnemySpawner.Instance != null ? EnemySpawner.Instance.ActiveEnemies : 0;

            GUI.Label(new Rect(16f, 10f, 250f, 30f), $"SCORE: {score:N0}", hudStyle);
            GUI.Label(new Rect(16f, 40f, 250f, 30f), $"WAVE: {wave}   ENEMIES: {alive}", hudStyle);

            if (EnemySpawner.Instance != null && EnemySpawner.Instance.WhaleIsActive)
            {
                var ws = new GUIStyle(hudStyle);
                ws.normal.textColor = new Color(1f, 0.3f, 1f);
                ws.fontSize = 18;
                GUI.Label(new Rect(Screen.width * 0.5f - 100f, 10f, 200f, 30f), "WHALE BOSS!", ws);
            }

            // ESC to pause
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
                GameManager.Instance.PauseGame();
        }

        void DrawPause()
        {
            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.color = new Color(0f, 0f, 0f, 0.5f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(cx - 150f, cy - 80f, 300f, 60f), "PAUSED", titleStyle);

            if (GUI.Button(new Rect(cx - 100f, cy, 200f, 45f), "RESUME", buttonStyle))
                GameManager.Instance.ResumeGame();

            if (GUI.Button(new Rect(cx - 100f, cy + 55f, 200f, 45f), "MAIN MENU", buttonStyle))
            {
                GameManager.Instance.ResumeGame();
                GameManager.Instance.TriggerGameOver();
            }
        }

        void DrawGameOver()
        {
            DrawEndScreen("GAME OVER", new Color(0.9f, 0.2f, 0.2f));
        }

        void DrawVictory()
        {
            DrawEndScreen("VICTORY!", new Color(0.9f, 0.85f, 0.1f));
        }

        void DrawEndScreen(string headline, Color headlineColor)
        {
            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var hs = new GUIStyle(titleStyle);
            hs.normal.textColor = headlineColor;
            GUI.Label(new Rect(cx - 220f, cy - 140f, 440f, 80f), headline, hs);

            GUI.Label(new Rect(cx - 200f, cy - 55f, 400f, 40f),
                $"SCORE: {GameManager.Instance.CurrentScore:N0}", bodyStyle);

            if (GUI.Button(new Rect(cx - 100f, cy + 10f, 200f, 50f), "PLAY AGAIN", buttonStyle))
                GameManager.Instance.StartGame();
        }

        // ── Style init ────────────────────────────────────────────────────

        void EnsureStyles()
        {
            if (stylesInit) return;
            stylesInit = true;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 48,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            titleStyle.normal.textColor = Color.white;

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 14,
                alignment = TextAnchor.MiddleCenter,
            };
            bodyStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize  = 18,
                fontStyle = FontStyle.Bold,
            };

            hudStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = 16,
                fontStyle = FontStyle.Bold,
            };
            hudStyle.normal.textColor = Color.white;
        }

        void Update()
        {
            if (GameManager.Instance?.State == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.PauseGame();
        }
    }
}
