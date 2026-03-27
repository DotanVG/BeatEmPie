using UnityEngine;

namespace BeatEmPie
{
    public enum GameState { MainMenu, Playing, Paused, Victory, GameOver }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Music Clips")]
        [SerializeField] AudioClip trackMainMenu;
        [SerializeField] AudioClip trackVictory;
        [SerializeField] AudioClip trackGameOver;

        [Header("References")]
        [SerializeField] DynamicMusicController dynamicMusic;

        public GameState State       { get; private set; } = GameState.MainMenu;
        public int       CurrentScore { get; private set; }
        public int       CurrentWave  { get; private set; }

        public event System.Action<GameState> OnStateChanged;
        public event System.Action<int>       OnScoreChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            SetState(GameState.MainMenu);
        }

        // ── Public API ────────────────────────────────────────────────────

        public void StartGame()
        {
            CurrentScore = 0;
            CurrentWave  = 0;
            OnScoreChanged?.Invoke(CurrentScore);

            SetState(GameState.Playing);

            dynamicMusic?.StartGameplayMusic();
            EnemySpawner.Instance?.StartSpawning();
        }

        public void PauseGame()
        {
            if (State != GameState.Playing) return;
            Time.timeScale = 0f;
            AudioManager.Instance?.PauseMusic();
            SetState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (State != GameState.Paused) return;
            Time.timeScale = 1f;
            AudioManager.Instance?.ResumeMusic();
            SetState(GameState.Playing);
        }

        public void TriggerVictory()
        {
            EnemySpawner.Instance?.StopSpawning();
            AudioManager.Instance?.CrossFade(trackVictory, 1f);
            SetState(GameState.Victory);
        }

        public void TriggerGameOver()
        {
            EnemySpawner.Instance?.StopSpawning();
            AudioManager.Instance?.CrossFade(trackGameOver, 1f);
            SetState(GameState.GameOver);
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void OnWaveComplete()
        {
            CurrentWave++;
            EnemySpawner.Instance?.StartSpawning();
        }

        // ── Internal ──────────────────────────────────────────────────────

        void SetState(GameState next)
        {
            State = next;
            OnStateChanged?.Invoke(next);

            if (next == GameState.MainMenu)
                AudioManager.Instance?.CrossFade(trackMainMenu, 1f);
        }
    }
}
