using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton audio manager — handles all SFX and music playback.
/// Noam's audio assets will be routed through this system.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager Instance;

    // AudioSource sfxSource
    // AudioSource musicSource

    // Dictionary<string, AudioClip> soundLibrary — named sound lookup

    // float sfxVolume
    // float musicVolume

    // Awake: set up singleton

    // PlaySFX(string name): play a named sound effect
    // PlaySFX(AudioClip clip): play a direct clip

    // PlayMusic(AudioClip track): start background music
    // StopMusic(): stop current music

    // SetSFXVolume(float volume)
    // SetMusicVolume(float volume)
}
