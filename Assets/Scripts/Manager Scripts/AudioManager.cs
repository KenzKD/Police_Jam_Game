using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static AudioManager Instance;

    // Audio mixer for managing audio channels
    public AudioMixer Mixer;

    // Arrays of background music and sound effects
    public Sound[] bgm, sfx;

    // Audio sources for background music and sound effects
    public AudioSource bgmSource, menuSource;

    public GameObject sfxParent, sfxLoopingParent;

    private List<AudioSource> sfxSources, sfxLoopingSources;

    private int sfxIndex = 0, sfxLoopingIndex = 0;

    // Initialize the audio manager
    private void Awake()
    {
        Instance = this; // Set in Awake() for Bgm and Sfx volume preferences

        sfxSources = sfxParent.GetComponentsInChildren<AudioSource>().ToList();
        sfxLoopingSources = sfxLoopingParent.GetComponentsInChildren<AudioSource>().ToList();
    }

    // Start playing the BGM
    private void Start()
    {
        bgmSource.Stop();
        PlayBGM("Theme");
    }

    // Play a background music track by name
    public void PlayBGM(string name)
    {
        Sound s = Array.Find(bgm, x => x.name == name);
        if (s == null)
        {
            Debug.Log($"Sound '{name}' not found!");
        }
        else
        {
            bgmSource.clip = s.clip;
            bgmSource.Play();
        }
    }

    // Play a sound effect by name
    public void PlaySFX(string name, bool sfxAllowOverlap = false, bool RandomizePitch = true)
    {
        Sound sound = Array.Find(sfx, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        if (!sfxAllowOverlap)
        {
            StopSFX();
        }

        if (RandomizePitch)
        {
            sfxSources[sfxIndex].pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        }
        else
        {
            sfxSources[sfxIndex].pitch = 1f;
        }

        sfxSources[sfxIndex].clip = sound.clip;
        sfxSources[sfxIndex].Play();
        sfxIndex = (sfxIndex + 1) % sfxSources.Count;
    }

    public void PlayMenuSFX(string name)
    {
        Sound sound = Array.Find(sfx, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        menuSource.PlayOneShot(sound.clip);
    }

    // Play a sound effect by name
    public void PlayLoopingSFX(string name, bool sfxAllowOverlap = false)
    {
        Sound sound = Array.Find(sfx, s => s.name == name);
        if (sound == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        if (!sfxAllowOverlap)
        {
            StopLoopingSFX();
        }

        sfxLoopingSources[sfxLoopingIndex].clip = sound.clip;
        sfxLoopingSources[sfxLoopingIndex].Play();
        sfxLoopingIndex = (sfxLoopingIndex + 1) % sfxLoopingSources.Count;
    }

    // Toggle looping of sfxSource
    public void StopLoopingSFX()
    {
        sfxLoopingSources.All(s => { s.Stop(); return true; });
    }

    // Stop sfxSource
    public void StopSFX()
    {
        sfxSources.All(s => { s.Stop(); return true; });
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // Adjust the volume of the background music
    public void BGMVolume(float volume)
    {
        // Ensure volume is not zero to avoid log(0) error
        volume = Mathf.Max(volume, 0.0001f);

        // Convert linear volume to decibels
        float decibels = 20f * Mathf.Log10(volume);

        Mixer.SetFloat("bgmMixerVolume", decibels);
    }

    // Adjust the volume of the sound effects
    public void SFXVolume(float volume)
    {
        // Ensure volume is not zero to avoid log(0) error
        volume = Mathf.Max(volume, 0.0001f);

        // Convert linear volume to decibels
        float decibels = 20f * Mathf.Log10(volume);

        Mixer.SetFloat("sfxMixerVolume", decibels);
    }
}
