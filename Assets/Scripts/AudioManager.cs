using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (musicSource == null)
        {
            Debug.LogError("Audio Source not assigned!");
            return;
        }

        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogError("No music clips assigned!");
            return;
        }

        //Start music
        musicSource.clip = musicClips[0];
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Music Settings
    //Contains all music clips
    //Music volume

    public float musicVol = 1f;
    public AudioSource musicSource; // Assign in the Inspector
    public AudioClip[] musicClips; // Assign in the Inspector

    public void PlayMusic(int musicID = 0)
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.clip = musicClips[musicID];
            musicSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVol = volume;

        if (musicSource != null)
        {
            musicSource.volume = musicVol;
        }
    }

    public void NextMusicClip()
    {
        if (musicSource != null && musicClips != null && musicClips.Length > 1)
        {
            int currentClipIndex = System.Array.IndexOf(musicClips, musicSource.clip);
            int nextClipIndex = (currentClipIndex + 1) % musicClips.Length;
            musicSource.clip = musicClips[nextClipIndex];
            musicSource.Play();
        }
    }


    //SFX Settings
    //Contains references to the sfx to play at any point
    //SFX volume

    public float sfxVol = 0.5f;

    //Plays non 3D SFX
    public void PlaySFX(AudioClip clip, float volumeOverride = 1f)
    {
        // Use a temporary AudioSource to play the SFX
        GameObject tempGO = new GameObject("SFX");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        //Makes sound 2D (for UI)
        tempSource.spatialBlend = 0f;
        tempSource.volume = sfxVol * volumeOverride;
        tempSource.Play();
        Destroy(tempGO, clip.length);
    }

    //Plays SFX at point
    public void PlaySFX(AudioClip clip, Vector3 playPoint, float volumeOverride = 1f)
    {
        // Use a temporary AudioSource to play the SFX
        GameObject tempGO = new GameObject("SFX");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        //Makes sound 3D (in game SFX)
        tempSource.spatialBlend = 1f;
        tempSource.volume = sfxVol * volumeOverride;
        tempSource.gameObject.transform.position = playPoint;
        Destroy(tempGO, clip.length);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVol = volume;
    }

    //Dialogue Settings
    public float dialogueVol = 0.75f;
}
