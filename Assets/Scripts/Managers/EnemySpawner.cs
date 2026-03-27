using System.Collections.Generic;
using UnityEngine;

namespace BeatEmPie
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] GameObject fishEnemyPrefab;
        [SerializeField] GameObject whaleEnemyPrefab;

        [Header("Spawn Settings")]
        [SerializeField] float spawnInterval  = 3f;
        [SerializeField] Transform[] spawnPoints;

        [Header("Wave Config")]
        [SerializeField] int fishPerWave  = 4;
        [SerializeField] int whaleEveryN  = 3;   // spawn a whale every N waves

        public int CurrentWave    { get; private set; }
        public int ActiveEnemies  => activeEnemies.Count;
        public bool WhaleIsActive { get; private set; }

        public event System.Action<int> OnWaveStarted;
        public event System.Action      OnWaveCleared;
        public event System.Action      OnWhaleSpawned;

        readonly List<GameObject> activeEnemies = new();
        bool spawning;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void StartSpawning()
        {
            if (spawning) return;
            spawning = true;
            SpawnWave();
        }

        public void StopSpawning() => spawning = false;

        void SpawnWave()
        {
            if (!spawning) return;
            CurrentWave++;
            OnWaveStarted?.Invoke(CurrentWave);

            int count = fishPerWave + (CurrentWave - 1);
            for (int i = 0; i < count; i++)
                SpawnEnemy(fishEnemyPrefab);

            bool spawnWhale = whaleEnemyPrefab != null && CurrentWave % whaleEveryN == 0;
            if (spawnWhale)
            {
                SpawnEnemy(whaleEnemyPrefab);
                WhaleIsActive = true;
                OnWhaleSpawned?.Invoke();
            }
        }

        void SpawnEnemy(GameObject prefab)
        {
            if (prefab == null || spawnPoints == null || spawnPoints.Length == 0) return;
            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var go    = Instantiate(prefab, point.position, Quaternion.identity);
            activeEnemies.Add(go);
        }

        public void NotifyEnemyDied(GameObject enemy)
        {
            activeEnemies.Remove(enemy);

            if (enemy.GetComponent<WhaleEnemy>() != null)
                WhaleIsActive = false;

            if (activeEnemies.Count == 0)
            {
                OnWaveCleared?.Invoke();
                if (spawning)
                    Invoke(nameof(SpawnWave), spawnInterval);
            }
        }
    }
}
