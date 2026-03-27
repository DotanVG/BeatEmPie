using System.Collections;
using UnityEngine;

namespace BeatEmPie
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volume")]
        [Range(0f, 1f)] [SerializeField] float musicVolume = 0.6f;
        [Range(0f, 1f)] [SerializeField] float sfxVolume   = 1f;

        [Header("CrossFade")]
        [SerializeField] float defaultCrossFadeDuration = 1.5f;

        AudioSource musicSourceA;
        AudioSource musicSourceB;
        AudioSource activeMusicSource;
        AudioSource sfxSource;

        Coroutine crossFadeCoroutine;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Create four AudioSources: A+B for crossfading music, one for SFX
            musicSourceA = CreateSource("MusicA", loop: true,  volume: musicVolume);
            musicSourceB = CreateSource("MusicB", loop: true,  volume: 0f);
            sfxSource    = CreateSource("SFX",    loop: false, volume: sfxVolume);

            activeMusicSource = musicSourceA;
        }

        // ── Music ──────────────────────────────────────────────────────────

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            if (activeMusicSource.clip == clip && activeMusicSource.isPlaying) return;

            activeMusicSource.clip   = clip;
            activeMusicSource.volume = musicVolume;
            activeMusicSource.Play();
        }

        public void StopMusic()
        {
            musicSourceA.Stop();
            musicSourceB.Stop();
        }

        public void PauseMusic()  => activeMusicSource.Pause();
        public void ResumeMusic() => activeMusicSource.UnPause();

        public void CrossFade(AudioClip newClip, float duration = -1f)
        {
            if (newClip == null) return;
            if (activeMusicSource.clip == newClip && activeMusicSource.isPlaying) return;

            if (crossFadeCoroutine != null) StopCoroutine(crossFadeCoroutine);
            float dur = duration < 0f ? defaultCrossFadeDuration : duration;
            crossFadeCoroutine = StartCoroutine(DoCrossFade(newClip, dur));
        }

        IEnumerator DoCrossFade(AudioClip newClip, float duration)
        {
            var outgoing = activeMusicSource;
            var incoming = outgoing == musicSourceA ? musicSourceB : musicSourceA;

            incoming.clip   = newClip;
            incoming.volume = 0f;
            incoming.Play();
            activeMusicSource = incoming;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed        += Time.unscaledDeltaTime;
                float t         = elapsed / duration;
                incoming.volume = Mathf.Lerp(0f, musicVolume, t);
                outgoing.volume = Mathf.Lerp(musicVolume, 0f, t);
                yield return null;
            }

            incoming.volume = musicVolume;
            outgoing.Stop();
            outgoing.clip = null;
        }

        // ── SFX ───────────────────────────────────────────────────────────

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        // ── Volume ────────────────────────────────────────────────────────

        public void SetMusicVolume(float v)
        {
            musicVolume = Mathf.Clamp01(v);
            if (activeMusicSource.isPlaying) activeMusicSource.volume = musicVolume;
        }

        public void SetSFXVolume(float v) => sfxVolume = Mathf.Clamp01(v);

        // ── Helpers ───────────────────────────────────────────────────────

        AudioSource CreateSource(string label, bool loop, float volume)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform);
            var src    = go.AddComponent<AudioSource>();
            src.loop   = loop;
            src.volume = volume;
            src.playOnAwake = false;
            return src;
        }
    }
}
