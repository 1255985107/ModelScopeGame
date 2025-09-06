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

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmChantClip;        // 云端城宁静圣咏（背景音乐，教程阶段）
    [SerializeField] private AudioClip bgmCollapseClip;     // 崩塌变奏曲（背景音乐，崩塌阶段）
    [SerializeField] private AudioClip sfxDataBreakClip;    // 数据碎裂声（平台崩塌时播放的音效）
    [SerializeField] private AudioClip sfxArcClip;          // 电弧声（最终坠落或靠近断裂电线时播放）
    [SerializeField] private AudioClip windClip;            // 风声音效（环境风声，可用于坠落或高空场景）
    private AudioSource audioSource; // 用于播放音效

    [Header("Act Control")]
    private bool isGameOver = false;

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

        if (deathUI != null)
            deathUI.SetActive(false);

        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
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
        if (jumpTutorialUI != null)
            jumpTutorialUI.SetActive(true);
        Debug.Log("流程：跳跃教学阶段");
    }



    // 最终坠落触发
    public void FinalFallTrigger()
    {
        PlaySFX(sfxArcClip);
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

        StartCoroutine(ResetLevelAfterDelay(2f));
    }

    private void HandleSuccessfulJump()
    {
        if (isGameOver) return;

        playerController.DisableMovement();
        playerController.DisableJump();
    }

    IEnumerator ResetLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 音效播放辅助方法
    private void PlayBGM(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void StopBGM()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
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
}
