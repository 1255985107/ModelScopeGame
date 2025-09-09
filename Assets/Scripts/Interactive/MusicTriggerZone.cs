using UnityEngine;

/// <summary>
/// 音乐触发区域，用于在特定区域切换背景音乐
/// </summary>
public class MusicTriggerZone : BaseTriggerZone
{
    [Header("Music Settings")]
    public AudioClip newBackgroundMusic;
    public bool stopCurrentMusic = false;
    public bool resumePreviousMusic = false;
    
    private static AudioClip previousMusic;

    protected override void OnPlayerEnterZone(Collider2D player)
    {
        // 音乐切换通常不需要按键，直接触发
        requiresInput = false;
    }

    protected override void ExecuteEffect()
    {
        if (AudioManager.Singleton == null) return;

        if (stopCurrentMusic)
        {
            // 保存当前音乐以便恢复
            if (AudioManager.Singleton.IsMusicPlaying())
            {
                // 这里需要AudioManager提供获取当前音乐的方法
            }
            AudioManager.Singleton.StopMusic();
        }
        else if (resumePreviousMusic && previousMusic != null)
        {
            AudioManager.Singleton.PlayMusic(previousMusic);
        }
        else if (newBackgroundMusic != null)
        {
            AudioManager.Singleton.ChangeMusic(newBackgroundMusic);
        }

        Debug.Log($"Music trigger executed: {(newBackgroundMusic ? newBackgroundMusic.name : "Stop/Resume")}");
    }
}
