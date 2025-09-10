using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip introVideo;
	[SerializeField] private string nextSceneName = "MainMenu";
    
    [Header("UI Elements")]
    [SerializeField] private GameObject skipPrompt; // 显示"按任意键跳过"的UI
    [SerializeField] private Text skipText; // 可选：显示跳过提示文本
    [SerializeField] private CanvasGroup fadeCanvas; // 用于淡入淡出效果
    
    [Header("Settings")]
    [SerializeField] private float skipPromptDelay = 2f; // 多少秒后显示跳过提示
    [SerializeField] private float fadeInDuration = 1f; // 淡入时间
    [SerializeField] private float fadeOutDuration = 1f; // 淡出时间
    [SerializeField] private bool allowSkip = true; // 是否允许跳过
    [SerializeField] private bool skipVideoInEditor = false; // 在编辑器中是否跳过视频
    
    private bool hasVideoEnded = false;
    private bool isSkipping = false;
    private Coroutine skipPromptCoroutine;

    void Start()
    {
        // 检查是否应该跳过视频（比如在编辑器中测试）
        if (ShouldSkipVideo())
        {
            LoadNextScene();
            return;
        }
        
        InitializeVideoPlayer();
        StartCoroutine(FadeIn());
        
        if (allowSkip)
        {
            skipPromptCoroutine = StartCoroutine(ShowSkipPromptAfterDelay());
        }
    }
    
    private bool ShouldSkipVideo()
    {
        // 如果在编辑器中且设置了跳过，则不播放视频
        #if UNITY_EDITOR
        if (skipVideoInEditor)
        {
            Debug.Log("Skipping video in editor mode");
            return true;
        }
        #endif
        
        // 可以在这里添加其他跳过条件，比如：
        // - 玩家设置中关闭了介绍视频
        // - 不是第一次启动游戏
        // - 没有设置视频文件
        
        if (introVideo == null)
        {
            Debug.LogWarning("No intro video assigned, skipping to main menu");
            return true;
        }
        
        return false;
    }

    void Update()
    {
        // 检测任意输入来跳过视频
        if (allowSkip && !isSkipping && !hasVideoEnded)
        {
            if (Input.anyKeyDown || 
                Input.GetMouseButtonDown(0) || 
                Input.GetMouseButtonDown(1) || 
                Input.GetMouseButtonDown(2))
            {
                SkipVideo();
            }
        }
    }

    private void InitializeVideoPlayer()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            if (introVideo != null)
            {
                videoPlayer.clip = introVideo;
            }
            
            // 订阅视频结束事件
            videoPlayer.loopPointReached += OnVideoEnd;
            
            // 开始播放
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("VideoPlayer component not found!");
            LoadNextScene();
        }

        // 隐藏跳过提示
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(false);
        }
    }

    private IEnumerator ShowSkipPromptAfterDelay()
    {
        yield return new WaitForSeconds(skipPromptDelay);
        
        if (skipPrompt != null && !hasVideoEnded && !isSkipping)
        {
            skipPrompt.SetActive(true);
            
            // 可选：添加闪烁效果
            StartCoroutine(BlinkSkipPrompt());
        }
    }

    private IEnumerator BlinkSkipPrompt()
    {
        if (skipText == null) yield break;
        
        while (!hasVideoEnded && !isSkipping && skipPrompt.activeInHierarchy)
        {
            skipText.color = new Color(skipText.color.r, skipText.color.g, skipText.color.b, 1f);
            yield return new WaitForSeconds(0.5f);
            skipText.color = new Color(skipText.color.r, skipText.color.g, skipText.color.b, 0.3f);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SkipVideo()
    {
        if (isSkipping || hasVideoEnded) return;
        
        isSkipping = true;
        
        // 停止所有协程
        if (skipPromptCoroutine != null)
        {
            StopCoroutine(skipPromptCoroutine);
        }
        StopAllCoroutines();
        
        // 停止视频
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
        
        // 隐藏跳过提示
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(false);
        }
        
        StartCoroutine(FadeOutAndLoadScene());
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (hasVideoEnded) return;
        
        hasVideoEnded = true;
        
        // 停止所有协程
        StopAllCoroutines();
        
        // 隐藏跳过提示
        if (skipPrompt != null)
        {
            skipPrompt.SetActive(false);
        }
        
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeIn()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
                yield return null;
            }
            
            fadeCanvas.alpha = 0f;
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        if (fadeCanvas != null)
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            
            fadeCanvas.alpha = 1f;
        }
        
        yield return new WaitForSeconds(0.1f); // 短暂等待确保淡出完成
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next scene name is not set!");
        }
    }

    void OnDestroy()
    {
        // 清理事件订阅
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
