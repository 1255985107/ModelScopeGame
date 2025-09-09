using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject[] platformsToCollapse;
    [SerializeField] private GameObject deathUI;       // 死亡界面
    [SerializeField] private GameObject jumpTutorialUI; // 跳跃教程UI
    [SerializeField] private GameObject walkTutorialUI; // 行走教程UI

    [Header("Player Respawn")]
    [SerializeField] private Transform playerSpawnPoint; // 玩家重生点
    [SerializeField] private float respawnDelay = 2f;   // 重生延迟

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmChantClip;        // 云端城宁静圣咏（背景音乐，教程阶段）
    [SerializeField] private AudioClip bgmCollapseClip;     // 崩塌变奏曲（背景音乐，崩塌阶段）
    [SerializeField] private AudioClip sfxDataBreakClip;    // 数据碎裂声（平台崩塌时播放的音效）
    [SerializeField] private AudioClip sfxArcClip;          // 电弧声（最终坠落或靠近断裂电线时播放）
    [SerializeField] private AudioClip windClip;            // 风声音效（环境风声，可用于坠落或高空场景）
    private AudioSource environmentAudioSource; // 用于播放环境音效

    [Header("Act Control")]
    private bool isGameOver = false;
    private bool hasLearnedJump = false; // 新增：是否已学会跳跃

    void Start()
    {
        SetupReferences();
        SetupEvents();
        InitializeLevel();
    }

    private void SetupReferences()
    {
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // 设置默认重生点为玩家当前位置
        if (playerSpawnPoint == null && playerController != null)
        {
            GameObject spawnPoint = new GameObject("PlayerSpawnPoint");
            spawnPoint.transform.position = playerController.transform.position;
            playerSpawnPoint = spawnPoint.transform;
        }

        if (deathUI != null)
            deathUI.SetActive(false);

        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(false);

        environmentAudioSource = GetComponent<AudioSource>();
        if (environmentAudioSource == null)
            environmentAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void SetupEvents()
    {
        if (playerController != null)
        {
            playerController.onDeath.AddListener(HandlePlayerDeath);
            playerController.onSuccessfulJump.AddListener(HandleSuccessfulJump);
        }
    }

    private void InitializeLevel()
    {
        // 初始阶段：只能行走
        PlayBGM(bgmChantClip);
        playerController.DisableJump();
        playerController.EnableMovement();
        hasLearnedJump = false; // 初始化为未学会跳跃
    }

    // 只能行走阶段触发
    public void OnlyWalkTrigger()
    {
        PlayBGM(bgmCollapseClip);
        playerController.DisableJump();
        if (walkTutorialUI != null)
            walkTutorialUI.SetActive(false);
        Debug.Log("流程：只能行走阶段");
    }

    // 跳跃教学阶段触发
    public void JumpTutorialTrigger()
    {
        StopBGM();
        PlayBGM(bgmCollapseClip);
        playerController.EnableJump();
        hasLearnedJump = true; // 标记已学会跳跃
        Debug.Log("玩家已学会跳跃，跳跃功能已启用");
        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(true);
        Debug.Log("流程：跳跃教学阶段");
    }



    // 最终坠落触发
    public void FinalFallTrigger()
    {
        // 播放环境电弧声音效
        PlayEnvironmentSFX(sfxArcClip);
        
        playerController.DisableJump();
        playerController.DisableMovement();
        Debug.Log("流程：最终坠落阶段");
    }

    private void HandlePlayerDeath()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (deathUI != null)
            deathUI.SetActive(true);

        // 死亡音效由PlayerController自己播放，这里不需要重复播放
        
        StartCoroutine(RespawnPlayerAfterDelay(respawnDelay));
    }

    private void RespawnPlayer()
    {
        if (playerController == null) return;

        // 重置玩家位置
        if (playerSpawnPoint != null)
        {
            playerController.transform.position = playerSpawnPoint.position;
        }
        
        // 重置玩家状态
        Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
            playerRb.gravityScale = 1f; // 恢复重力
        }

        // 重新启用玩家控制
        playerController.EnableMovement();
        if (hasLearnedJump)
            playerController.EnableJump();

        // 隐藏死亡UI
        if (deathUI != null)
            deathUI.SetActive(false);

        // 重置游戏状态
        isGameOver = false;
        
        Debug.Log("Player respawned");
    }

    IEnumerator RespawnPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }

    private void HandleSuccessfulJump()
    {
        if (isGameOver) return;

        //playerController.DisableMovement();
        //playerController.DisableJump();
    }

    // 音效播放辅助方法
    private void PlayBGM(AudioClip clip)
    {
        AudioManager.Singleton.PlayMusic(clip);
        // if (audioSource != null)
        // {
        //     audioSource.loop = true;
        //     audioSource.clip = clip;
        //     audioSource.Play();
        // }
    }

    private void StopBGM()
    {
        AudioManager.Singleton.StopMusic();

        // if (audioSource != null)
        // {
        //     audioSource.Stop();
        //     audioSource.clip = null;
        // }
    }

    /// <summary>
    /// 播放环境音效（如平台崩塌、电弧声等）
    /// </summary>
    /// <param name="clip">要播放的音效</param>
    public void PlayEnvironmentSFX(AudioClip clip)
    {
        if (environmentAudioSource != null && clip != null)
        {
            environmentAudioSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// 播放平台崩塌音效
    /// </summary>
    public void PlayPlatformCollapseSound()
    {
        PlayEnvironmentSFX(sfxDataBreakClip);
    }

    /// <summary>
    /// 播放风声音效
    /// </summary>
    public void PlayWindSound()
    {
        PlayEnvironmentSFX(windClip);
    }

    public void OnTriggerZoneActivated()
    {
         if (walkTutorialUI != null && walkTutorialUI.activeSelf)
         {
            walkTutorialUI.SetActive(false);
            OnlyWalkTrigger();
          }
        else if (jumpTutorialUI != null && jumpTutorialUI.activeSelf)
        {
            jumpTutorialUI.SetActive(false);
            JumpTutorialTrigger();
        }
        Debug.Log("Tutorial Zone Completed!");
    }

    // 在Update或合适时机动态控制跳跃权限
    void Update()
    {
        if (playerController != null)
        {
            if (hasLearnedJump)
                playerController.EnableJump();
            else
                playerController.DisableJump();
        }
    }
}
