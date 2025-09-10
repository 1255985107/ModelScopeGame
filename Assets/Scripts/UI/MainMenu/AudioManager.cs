using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 简单的音频管理器，负责播放音效和管理音频设置
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton { get; private set; }
    
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHoverSound;
    
    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playMusicOnStart = true;
    
    // Audio mixer parameter names (should match SettingsMenuManager)
    private const string MUSIC_VOLUME_PARAM = "MusicVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    
    void Awake()
    {
        // Singleton pattern
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudioSources()
    {
        Debug.Assert(audioMixer != null, "AudioManager: Audio Mixer is not assigned!");
        // Create audio sources if not assigned
        if (musicSource == null)
        {
            Debug.Log("AudioManager: Music Source not assigned, creating one.");
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }
        if (sfxSource == null)
        {
            Debug.Log("AudioManager: SFX Source not assigned, creating one.");
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }
        // Play background music on start if enabled
        if (playMusicOnStart && backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }
    
    #region Music Methods
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="musicClip">音乐剪辑</param>
    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }
    
    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
    
    /// <summary>
    /// 切换背景音乐
    /// </summary>
    /// <param name="newClip">新的音乐剪辑</param>
    public void ChangeMusic(AudioClip newClip)
    {
        if (musicSource != null && newClip != null)
        {
            bool wasPlaying = musicSource.isPlaying;
            musicSource.Stop();
            musicSource.clip = newClip;
            if (wasPlaying)
            {
                musicSource.Play();
            }
            Debug.Log($"Music changed to {newClip.name}");
        }
    }
    
    /// <summary>
    /// 检查音乐是否正在播放
    /// </summary>
    /// <returns>是否正在播放</returns>
    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }
    
    #endregion
    
    #region SFX Methods
    
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="clip">音效剪辑</param>
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    /// <summary>
    /// 公共方法：播放音效
    /// </summary>
    /// <param name="clip">音效剪辑</param>
    public void PlaySFXClip(AudioClip clip)
    {
        PlaySFX(clip);
    }

    public void PlayVoiceClip(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.Stop();        // 先停止当前语音
            sfxSource.clip = clip;   // 设置新的剪辑
            sfxSource.Play();        // 播放
        }
    }

    /// <summary>
    /// 播放按钮点击音效
    /// </summary>
    private void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    
    /// <summary>
    /// 播放按钮悬停音效
    /// </summary>
    private void PlayButtonHover()
    {
        PlaySFX(buttonHoverSound);
    }
    
    #endregion
    
    #region Volume Control Methods
    
    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">音量 (0-1)</param>
    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert 0-1 range to decibel range (-80 to 0)
            float dbValue = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat(MUSIC_VOLUME_PARAM, dbValue);
        }
        else if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量 (0-1)</param>
    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            // Convert 0-1 range to decibel range (-80 to 0)
            float dbValue = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
            audioMixer.SetFloat(SFX_VOLUME_PARAM, dbValue);
        }
        else if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
    
    /// <summary>
    /// 获取音乐音量
    /// </summary>
    /// <returns>音量值 (0-1)</returns>
    public float GetMusicVolume()
    {
        if (audioMixer != null)
        {
            float dbValue;
            if (audioMixer.GetFloat(MUSIC_VOLUME_PARAM, out dbValue))
            {
                return dbValue <= -80f ? 0f : Mathf.Pow(10f, dbValue / 20f);
            }
        }
        else if (musicSource != null)
        {
            return musicSource.volume;
        }
        return 1f;
    }
    
    /// <summary>
    /// 获取音效音量
    /// </summary>
    /// <returns>音量值 (0-1)</returns>
    public float GetSFXVolume()
    {
        if (audioMixer != null)
        {
            float dbValue;
            if (audioMixer.GetFloat(SFX_VOLUME_PARAM, out dbValue))
            {
                return dbValue <= -80f ? 0f : Mathf.Pow(10f, dbValue / 20f);
            }
        }
        else if (sfxSource != null)
        {
            return sfxSource.volume;
        }
        return 1f;
    }

    #endregion

    /// <summary>
    /// 停止当前正在播放的音效
    /// </summary>
    public void StopCurrentSFX()
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }
}
