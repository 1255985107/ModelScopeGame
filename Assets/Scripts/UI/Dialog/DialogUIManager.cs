using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 对话UI管理器 - 负责对话界面显示
/// </summary>
public class DialogUIManager : MonoBehaviour
{
    [Header("UI组件引用")]
    [SerializeField] private GameObject dialogPanel;             // 对话面板
    [SerializeField] private TextMeshProUGUI characterNameText;  // 角色名字文本
    [SerializeField] private TextMeshProUGUI dialogText;         // 对话内容文本
    [SerializeField] private Image characterPortrait;            // 角色头像
    [SerializeField] private Button continueButton;              // 继续按钮（可选）
    [SerializeField] private Button skipButton;                  // 跳过按钮（可选）
    
    [Header("UI效果设置")]
    [SerializeField] private float panelFadeSpeed = 2f;          // 面板淡入淡出速度
    [SerializeField] private bool useTypewriterEffect = true;    // 是否使用打字机效果
    [SerializeField] private AudioClip typewriterSound;          // 打字音效
    [SerializeField] private float typewriterSoundInterval = 3;  // 打字音效间隔（每几个字符播放一次）
    
    [Header("样式设置")]
    [SerializeField] private Sprite defaultPortrait;             // 默认头像
    [SerializeField] private Color defaultTextColor = Color.white; // 默认文字颜色
    
    // 私有变量
    private CanvasGroup panelCanvasGroup;
    private Coroutine fadeCoroutine;
    private AudioManager audioManager;
    private int typewriterSoundCounter = 0;
    
    // 组件引用
    private DialogManager dialogManager;
    
    void Awake()
    {
        // 获取组件引用
        dialogManager = GetComponent<DialogManager>();
        if (dialogManager == null)
        {
            dialogManager = FindObjectOfType<DialogManager>();
        }
        
        audioManager = AudioManager.Singleton;
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        // 获取或添加CanvasGroup
        if (dialogPanel != null)
        {
            panelCanvasGroup = dialogPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = dialogPanel.AddComponent<CanvasGroup>();
            }
        }
        
        // 设置按钮事件
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
    }
    
    void Start()
    {
        // 初始化时隐藏对话面板
        HideDialogPanel(true);
        
        // 订阅对话管理器事件
        DialogManager.OnDialogStart += OnDialogStarted;
        DialogManager.OnDialogEnd += OnDialogEnded;
        DialogManager.OnDialogAdvance += OnDialogAdvanced;
    }
    
    void OnDestroy()
    {
        // 取消订阅事件
        DialogManager.OnDialogStart -= OnDialogStarted;
        DialogManager.OnDialogEnd -= OnDialogEnded;
        DialogManager.OnDialogAdvance -= OnDialogAdvanced;
    }
    
    #region 公共方法
    
    /// <summary>
    /// 显示对话面板
    /// </summary>
    /// <param name="immediate">是否立即显示</param>
    public void ShowDialogPanel(bool immediate = false)
    {
        if (dialogPanel == null) return;
        
        dialogPanel.SetActive(true);
        
        if (immediate)
        {
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 1f;
            }
        }
        else
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadePanel(true));
        }
    }
    
    /// <summary>
    /// 隐藏对话面板
    /// </summary>
    /// <param name="immediate">是否立即隐藏</param>
    public void HideDialogPanel(bool immediate = false)
    {
        if (dialogPanel == null) return;
        
        if (immediate)
        {
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0f;
            }
            dialogPanel.SetActive(false);
        }
        else
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadePanel(false));
        }
    }
    
    /// <summary>
    /// 设置角色名字
    /// </summary>
    /// <param name="name">角色名字</param>
    public void SetCharacterName(string name)
    {
        if (characterNameText != null)
        {
            characterNameText.text = name;
            characterNameText.gameObject.SetActive(!string.IsNullOrEmpty(name));
        }
    }
    
    /// <summary>
    /// 设置对话文本
    /// </summary>
    /// <param name="text">对话文本</param>
    public void SetDialogText(string text)
    {
        if (dialogText != null)
        {
            dialogText.text = text;
            
            // 播放打字音效
            if (useTypewriterEffect && typewriterSound != null && audioManager != null)
            {
                typewriterSoundCounter++;
                if (typewriterSoundCounter >= typewriterSoundInterval)
                {
                    audioManager.PlaySFXClip(typewriterSound);
                    typewriterSoundCounter = 0;
                }
            }
        }
    }
    
    /// <summary>
    /// 清空对话文本
    /// </summary>
    public void ClearDialogText()
    {
        if (dialogText != null)
        {
            dialogText.text = "";
        }
        typewriterSoundCounter = 0;
    }
    
    /// <summary>
    /// 设置角色头像
    /// </summary>
    /// <param name="portrait">头像精灵</param>
    public void SetCharacterPortrait(Sprite portrait)
    {
        if (characterPortrait != null)
        {
            if (portrait != null)
            {
                characterPortrait.sprite = portrait;
                characterPortrait.gameObject.SetActive(true);
            }
            else
            {
                characterPortrait.sprite = defaultPortrait;
                characterPortrait.gameObject.SetActive(defaultPortrait != null);
            }
        }
    }
    
    /// <summary>
    /// 设置文本颜色
    /// </summary>
    /// <param name="color">文本颜色</param>
    public void SetTextColor(Color color)
    {
        if (dialogText != null)
        {
            dialogText.color = color;
        }
    }
    
    /// <summary>
    /// 重置UI到默认状态
    /// </summary>
    public void ResetToDefault()
    {
        SetCharacterName("");
        ClearDialogText();
        SetCharacterPortrait(null);
        SetTextColor(defaultTextColor);
    }
    
    #endregion
    
    #region 私有方法
    
    /// <summary>
    /// 面板淡入淡出协程
    /// </summary>
    /// <param name="fadeIn">是否淡入</param>
    /// <returns></returns>
    private IEnumerator FadePanel(bool fadeIn)
    {
        if (panelCanvasGroup == null) yield break;
        
        float startAlpha = panelCanvasGroup.alpha;
        float targetAlpha = fadeIn ? 1f : 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < 1f / panelFadeSpeed)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime * panelFadeSpeed;
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        panelCanvasGroup.alpha = targetAlpha;
        
        if (!fadeIn)
        {
            dialogPanel.SetActive(false);
        }
    }
    
    #endregion
    
    #region 事件处理
    
    /// <summary>
    /// 对话开始事件处理
    /// </summary>
    private void OnDialogStarted()
    {
        ResetToDefault();
    }
    
    /// <summary>
    /// 对话结束事件处理
    /// </summary>
    private void OnDialogEnded()
    {
        // 对话结束时可以做一些清理工作
    }
    
    /// <summary>
    /// 对话推进事件处理
    /// </summary>
    /// <param name="dialogData">当前对话数据</param>
    private void OnDialogAdvanced(DialogData dialogData)
    {
        // 可以根据对话数据做一些特殊处理
        // 比如改变UI样式、播放特效等
    }
    
    /// <summary>
    /// 继续按钮点击处理
    /// </summary>
    private void OnContinueButtonClicked()
    {
        if (dialogManager != null)
        {
            dialogManager.AdvanceDialog();
        }
    }
    
    /// <summary>
    /// 跳过按钮点击处理
    /// </summary>
    private void OnSkipButtonClicked()
    {
        if (dialogManager != null)
        {
            dialogManager.SkipDialog();
        }
    }
    
    #endregion
    
    #region 调试和工具方法
    
    /// <summary>
    /// 验证UI组件设置是否完整
    /// </summary>
    /// <returns>验证结果</returns>
    public bool ValidateUIComponents()
    {
        bool isValid = true;
        
        if (dialogPanel == null)
        {
            Debug.LogError("DialogUIManager: 对话面板未设置");
            isValid = false;
        }
        
        if (dialogText == null)
        {
            Debug.LogError("DialogUIManager: 对话文本组件未设置");
            isValid = false;
        }
        
        if (characterNameText == null)
        {
            Debug.LogWarning("DialogUIManager: 角色名字文本组件未设置");
        }
        
        if (characterPortrait == null)
        {
            Debug.LogWarning("DialogUIManager: 角色头像组件未设置");
        }
        
        return isValid;
    }
    
    #endregion
}
