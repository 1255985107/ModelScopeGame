using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 对话管理器 - 负责对话逻辑控制
/// </summary>
public class DialogManager : MonoBehaviour
{
    [Header("对话配置")]
    [SerializeField] private DialogSequence currentDialog;
    [SerializeField] private bool startDialogOnAwake = false;
    
    [Header("输入设置")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private KeyCode alternativeInteractKey = KeyCode.Space;
    
    // 对话状态
    private int currentDialogIndex = 0;
    private bool isDialogActive = false;
    private bool isTextDisplaying = false;
    private bool canAdvance = false;
    
    // 组件引用
    private DialogUIManager dialogUI;
    private AudioManager audioManager;
    private Coroutine currentDialogCoroutine;
    private Coroutine textDisplayCoroutine;
    
    // 事件
    public static event Action OnDialogStart;
    public static event Action OnDialogEnd;
    public static event Action<DialogData> OnDialogAdvance;
    
    // 单例访问
    public static DialogManager Instance { get; private set; }
    
    void Awake()
    {
        // 简单单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 获取组件
        dialogUI = GetComponent<DialogUIManager>();
        if (dialogUI == null)
        {
            dialogUI = FindObjectOfType<DialogUIManager>();
        }
        
        audioManager = AudioManager.Singleton;
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
    }
    
    void Start()
    {
        // 启用输入监听
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPressed;
            interactAction.action.Enable();
        }
        
        // 如果设置了自动开始对话
        if (startDialogOnAwake && currentDialog != null)
        {
            StartDialog(currentDialog);
        }
    }
    
    void Update()
    {
        // 备用按键检测
        if (Input.GetKeyDown(alternativeInteractKey))
        {
            OnInteractPressed(new InputAction.CallbackContext());
        }
    }
    
    void OnDestroy()
    {
        // 清理输入监听
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPressed;
        }
    }
    
    /// <summary>
    /// 开始对话序列
    /// </summary>
    /// <param name="DialogSequence">对话序列</param>
    public void StartDialog(DialogSequence DialogSequence)
    {
        if (DialogSequence == null || DialogSequence.dialogs.Length == 0)
        {
            Debug.LogWarning("DialogManager: 对话序列为空或没有对话内容");
            return;
        }
        
        if (isDialogActive)
        {
            Debug.LogWarning("DialogManager: 对话已经在进行中");
            return;
        }
        
        currentDialog = DialogSequence;
        currentDialogIndex = 0;
        isDialogActive = true;
        canAdvance = false;
        
        // 暂停游戏（如果设置了）
        if (currentDialog.pauseGameDuringDialog)
        {
            Time.timeScale = 0f;
        }
        
        // 显示对话UI
        if (dialogUI != null)
        {
            dialogUI.ShowDialogPanel();
        }
        
        // 触发事件
        OnDialogStart?.Invoke();
        
        // 开始显示第一个对话
        ShowCurrentDialog();
    }
    
    /// <summary>
    /// 结束对话
    /// </summary>
    public void EndDialog()
    {
        if (!isDialogActive) return;
        
        // 停止所有协程
        if (currentDialogCoroutine != null)
        {
            StopCoroutine(currentDialogCoroutine);
        }
        if (textDisplayCoroutine != null)
        {
            StopCoroutine(textDisplayCoroutine);
        }
        
        isDialogActive = false;
        isTextDisplaying = false;
        canAdvance = false;
        
        // 恢复游戏时间
        if (currentDialog != null && currentDialog.pauseGameDuringDialog)
        {
            Time.timeScale = 1f;
        }
        
        // 隐藏对话UI
        if (dialogUI != null)
        {
            dialogUI.HideDialogPanel();
        }
        
        // 触发事件
        OnDialogEnd?.Invoke();
        
        currentDialog = null;
    }
    
    /// <summary>
    /// 显示当前对话
    /// </summary>
    private void ShowCurrentDialog()
    {
        if (!isDialogActive || currentDialog == null || currentDialogIndex >= currentDialog.dialogs.Length)
        {
            // 检查是否循环播放
            if (currentDialog != null && currentDialog.loopSequence)
            {
                currentDialogIndex = 0;
            }
            else
            {
                EndDialog();
                return;
            }
        }
        
        DialogData dialogData = currentDialog.dialogs[currentDialogIndex];
        
        // 更新UI显示
        if (dialogUI != null)
        {
            dialogUI.SetCharacterName(dialogData.characterName);
            dialogUI.SetCharacterPortrait(dialogData.characterPortrait);
            dialogUI.SetTextColor(dialogData.textColor);
        }
        
        // 播放语音
        if (dialogData.voiceClip != null && audioManager != null)
        {
            audioManager.PlaySFXClip(dialogData.voiceClip);
        }
        
        // 开始显示文字
        if (textDisplayCoroutine != null)
        {
            StopCoroutine(textDisplayCoroutine);
        }
        textDisplayCoroutine = StartCoroutine(DisplayTextCoroutine(dialogData));
        
        // 触发事件
        OnDialogAdvance?.Invoke(dialogData);
    }
    
    /// <summary>
    /// 文字显示协程
    /// </summary>
    /// <param name="dialogData">对话数据</param>
    /// <returns></returns>
    private IEnumerator DisplayTextCoroutine(DialogData dialogData)
    {
        isTextDisplaying = true;
        canAdvance = false;
        
        if (dialogUI != null)
        {
            dialogUI.ClearDialogText();
        }
        
        string fullText = dialogData.dialogText;
        
        // 逐字显示文本
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (dialogUI != null)
            {
                dialogUI.SetDialogText(fullText.Substring(0, i));
            }
            
            yield return new WaitForSecondsRealtime(dialogData.textDisplaySpeed);
        }
        
        isTextDisplaying = false;
        canAdvance = true;
        
        // 如果不等待输入，自动继续
        if (!dialogData.waitForInput)
        {
            yield return new WaitForSecondsRealtime(dialogData.autoAdvanceDelay);
            AdvanceDialog();
        }
    }
    
    /// <summary>
    /// 前进到下一个对话
    /// </summary>
    public void AdvanceDialog()
    {
        if (!isDialogActive) return;
        
        // 如果文字还在显示中，立即显示完整文字
        if (isTextDisplaying)
        {
            if (textDisplayCoroutine != null)
            {
                StopCoroutine(textDisplayCoroutine);
            }
            
            // 显示完整文字
            if (currentDialog != null && currentDialogIndex < currentDialog.dialogs.Length)
            {
                DialogData currentData = currentDialog.dialogs[currentDialogIndex];
                if (dialogUI != null)
                {
                    dialogUI.SetDialogText(currentData.dialogText);
                }
            }
            
            isTextDisplaying = false;
            canAdvance = true;
            return;
        }
        
        // 如果可以继续，前进到下一个对话
        if (canAdvance)
        {
            currentDialogIndex++;
            ShowCurrentDialog();
        }
    }
    
    /// <summary>
    /// 输入处理
    /// </summary>
    /// <param name="context">输入上下文</param>
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isDialogActive)
        {
            AdvanceDialog();
        }
    }
    
    /// <summary>
    /// 检查是否正在进行对话
    /// </summary>
    /// <returns>是否正在对话</returns>
    public bool IsDialogActive()
    {
        return isDialogActive;
    }
    
    /// <summary>
    /// 获取当前对话进度
    /// </summary>
    /// <returns>当前对话索引和总对话数</returns>
    public (int current, int total) GetDialogProgress()
    {
        if (currentDialog == null) return (0, 0);
        return (currentDialogIndex + 1, currentDialog.dialogs.Length);
    }
    
    /// <summary>
    /// 跳过当前对话序列
    /// </summary>
    public void SkipDialog()
    {
        if (isDialogActive)
        {
            EndDialog();
        }
    }
    
    /// <summary>
    /// 暂停/恢复对话
    /// </summary>
    public void TogglePauseDialog()
    {
        if (!isDialogActive) return;
        
        if (textDisplayCoroutine != null)
        {
            // 这里可以实现暂停逻辑
            // Unity的协程不能直接暂停，需要自己实现状态控制
        }
    }
}
