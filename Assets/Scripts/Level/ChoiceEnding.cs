using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ChoiceEnding : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string videoPath;
    
    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choice1Button;
    [SerializeField] private Button choice2Button;
    [SerializeField] private Button choice3Button;
    
    [Header("Button Text (Optional)")]
    [SerializeField] private TextMeshProUGUI choice1Text;
    [SerializeField] private TextMeshProUGUI choice2Text;
    [SerializeField] private TextMeshProUGUI choice3Text;
    
    [Header("Scene Settings")]
    [SerializeField] private string choice1SceneName;
    [SerializeField] private string choice2SceneName;
    [SerializeField] private string choice3SceneName;
    
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeOutDuration = 1f;
    
    private bool isShowingChoices = false;
    
    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        
        InitializeVideoPlayer();
        
        SetupButtons();
        
        ShowChoices();
    }
    
    private void InitializeVideoPlayer()
    {
        if (videoPlayer != null && !string.IsNullOrEmpty(videoPath))
        {
            videoPlayer.source = VideoSource.Url;
            
            string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);
            videoPlayer.url = fullPath;
            
            Debug.Log($"Loading video from: {fullPath}");
            
            videoPlayer.isLooping = true;
            videoPlayer.loopPointReached += OnVideoEnd;
            
            videoPlayer.Play();
        }
        else if (string.IsNullOrEmpty(videoPath))
        {
            Debug.LogWarning("Video path is not set in ChoiceEnding");
        }
    }
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video ended, showing choices");
        ShowChoices();
    }
    
    private void SetupButtons()
    {
        if (choice1Button != null)
        {
            choice1Button.onClick.AddListener(() => OnChoiceSelected(1));
        }
        
        if (choice2Button != null)
        {
            choice2Button.onClick.AddListener(() => OnChoiceSelected(2));
        }
        
        if (choice3Button != null)
        {
            choice3Button.onClick.AddListener(() => OnChoiceSelected(3));
        }
    }
    
    public void ShowChoices()
    {
        if (isShowingChoices) return;
        
        isShowingChoices = true;
        
        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        
        Debug.Log("Showing choice panel");
    }
    
    private void OnChoiceSelected(int choiceNumber)
    {
        string targetScene = "";
        
        switch (choiceNumber)
        {
            case 1:
                targetScene = choice1SceneName;
                Debug.Log($"Choice 1 selected: {targetScene}");
                break;
            case 2:
                targetScene = choice2SceneName;
                Debug.Log($"Choice 2 selected: {targetScene}");
                break;
            case 3:
                targetScene = choice3SceneName;
                Debug.Log($"Choice 3 selected: {targetScene}");
                break;
        }
        
        if (!string.IsNullOrEmpty(targetScene))
        {
            if (choicePanel != null)
            {
                choicePanel.SetActive(false);
            }
            
            StartCoroutine(FadeOutAndLoadScene(targetScene));
        }
        else
        {
            Debug.LogError($"Scene name for choice {choiceNumber} is not set!");
        }
    }
    
    private IEnumerator FadeOutAndLoadScene(string sceneName)
    {
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = false;
            videoPlayer.Stop();
        }
        
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
        
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    
    public void SetChoiceTexts(string text1, string text2, string text3)
    {
        if (choice1Text != null) choice1Text.text = text1;
        if (choice2Text != null) choice2Text.text = text2;
        if (choice3Text != null) choice3Text.text = text3;
    }
    
    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
