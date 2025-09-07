using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Singleton;
    [Tooltip("Background music audio source")]
    public AudioClip backgroundMusic;

    [Tooltip("Music Volume")]
    [Range(0f, 2f)]
    public float musicVolume = 0.5f;

    [Tooltip("Is music looping")]
    public bool isLooping = true;

    [Tooltip("Play music on start")]
    public bool playOnStart = true;

    private AudioSource audioSource;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "AudioSource component is missing on MusicManager GameObject");
        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.loop = isLooping;
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayMusic()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or backgroundMusic is null, cannot play music.");
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            musicVolume = Mathf.Clamp01(volume);
            audioSource.volume = musicVolume;
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource != null && newClip != null)
        {
            bool wasPlaying = audioSource.isPlaying;
            audioSource.Stop();
            audioSource.clip = newClip;
            if (wasPlaying)
            {
                audioSource.Play();
            }
            Debug.Log($"Music changed to {newClip.name}");
        }
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    static public MusicManager GetSingleton()
    {
        return Singleton;
    }
}
