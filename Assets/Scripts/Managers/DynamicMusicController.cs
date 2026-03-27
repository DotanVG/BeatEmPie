using UnityEngine;

namespace BeatEmPie
{
    /// <summary>
    /// Monitors enemy state and drives music transitions automatically.
    /// Requires AudioManager and EnemySpawner singletons to be present.
    /// </summary>
    public class DynamicMusicController : MonoBehaviour
    {
        [Header("Music Clips")]
        [SerializeField] AudioClip trackGameplayCalm;
        [SerializeField] AudioClip trackGameplayIntense;
        [SerializeField] AudioClip trackBossFight;

        [Header("Thresholds")]
        [SerializeField] int intenseEnemyThreshold = 5;
        [SerializeField] float checkInterval = 2f;

        MusicTrack currentTrack = MusicTrack.None;
        float checkTimer;

        void OnEnable()
        {
            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.OnWhaleSpawned += OnWhaleSpawned;
                EnemySpawner.Instance.OnWaveCleared  += OnWaveCleared;
            }
        }

        void OnDisable()
        {
            if (EnemySpawner.Instance != null)
            {
                EnemySpawner.Instance.OnWhaleSpawned -= OnWhaleSpawned;
                EnemySpawner.Instance.OnWaveCleared  -= OnWaveCleared;
            }
        }

        void Update()
        {
            // Don't override boss or non-gameplay tracks
            if (currentTrack == MusicTrack.BossFight) return;
            if (currentTrack != MusicTrack.GameplayCalm &&
                currentTrack != MusicTrack.GameplayIntense) return;

            checkTimer -= Time.deltaTime;
            if (checkTimer > 0f) return;
            checkTimer = checkInterval;

            EvaluateIntensity();
        }

        void EvaluateIntensity()
        {
            if (EnemySpawner.Instance == null) return;

            bool shouldBeIntense = EnemySpawner.Instance.ActiveEnemies >= intenseEnemyThreshold;
            MusicTrack target = shouldBeIntense ? MusicTrack.GameplayIntense : MusicTrack.GameplayCalm;

            if (target != currentTrack)
                SwitchTo(target);
        }

        public void StartGameplayMusic()
        {
            SwitchTo(MusicTrack.GameplayCalm);
        }

        void SwitchTo(MusicTrack track)
        {
            currentTrack = track;
            var clip = ClipFor(track);
            AudioManager.Instance?.CrossFade(clip);
        }

        void OnWhaleSpawned()  => SwitchTo(MusicTrack.BossFight);

        void OnWaveCleared()
        {
            if (currentTrack == MusicTrack.BossFight)
                SwitchTo(MusicTrack.GameplayCalm);
        }

        AudioClip ClipFor(MusicTrack track) => track switch
        {
            MusicTrack.GameplayCalm    => trackGameplayCalm,
            MusicTrack.GameplayIntense => trackGameplayIntense,
            MusicTrack.BossFight       => trackBossFight,
            _                          => null
        };
    }
}
