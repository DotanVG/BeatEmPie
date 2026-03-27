using UnityEngine;

/// <summary>
/// Central game manager — singleton that controls overall game state.
/// Tracks score, current wave, and holds references to core systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance;

    // Current game state
    // GameState: MainMenu, Playing, Paused, GameOver

    // int currentScore
    // int currentWave

    // Reference to EnemySpawner
    // Reference to PieInventory

    // Awake: set up singleton pattern

    // StartGame(): transition to Playing state, begin first wave

    // PauseGame() / ResumeGame(): toggle pause state

    // GameOver(): trigger game over sequence

    // AddScore(int amount): add to score, update UI

    // OnWaveComplete(): notify spawner to begin next wave
}
