using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 对话触发器 - 用于触发对话的组件
/// </summary>
public class DialogTrigger : MonoBehaviour
{
    [Header("触发设置")]
    [SerializeField] private DialogSequence dialogToPlay;    // 要播放的对话序列
    [SerializeField] private TriggerType triggerType = TriggerType.OnTriggerEnter; // 触发方式
    [SerializeField] private bool oneTimeOnly = false;         // 是否只触发一次
    [SerializeField] private string playerTag = "Player";      // 玩家标签
    
    [Header("交互设置")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;  // 交互按键
    [SerializeField] private InputActionReference interactAction; // 输入动作引用
    [SerializeField] private GameObject interactPrompt;        // 交互提示UI
    
    [Header("调试")]
    [SerializeField] private bool debugMode = false;
    
    // 私有变量
    private bool hasTriggered = false;
    private bool playerInRange = false;
    private DialogManager dialogManager;
    
    // 触发方式枚举
    public enum TriggerType
    {
        OnTriggerEnter,     // 碰撞触发
        OnInteract,         // 交互触发
        Manual              // 手动触发
    }
    
    void Start()
    {
        dialogManager = DialogManager.Instance;
        if (dialogManager == null)
        {
            dialogManager = FindObjectOfType<DialogManager>();
        }
        
        if (dialogManager == null)
        {
            Debug.LogError($"DialogTrigger ({name}): 找不到 DialogManager！");
        }
        
        // 启用输入动作
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteractPressed;
            interactAction.action.Enable();
        }
        
        // 初始化时隐藏交互提示
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 初始化完成，触发类型：{triggerType}");
        }
    }
    
    void Update()
    {
        // 检查传统按键输入
        if (playerInRange && triggerType == TriggerType.OnInteract)
        {
            if (Input.GetKeyDown(interactKey))
            {
                TriggerDialog();
            }
        }
    }
    
    void OnDestroy()
    {
        // 清理输入动作
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPressed;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 玩家进入触发区域");
        }
        
        playerInRange = true;
        
        if (triggerType == TriggerType.OnTriggerEnter)
        {
            TriggerDialog();
        }
        else if (triggerType == TriggerType.OnInteract)
        {
            ShowInteractPrompt(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 玩家离开触发区域");
        }
        
        playerInRange = false;
        
        if (triggerType == TriggerType.OnInteract)
        {
            ShowInteractPrompt(false);
        }
    }
    
    /// <summary>
    /// 输入动作回调
    /// </summary>
    /// <param name="context"></param>
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (playerInRange && triggerType == TriggerType.OnInteract)
        {
            TriggerDialog();
        }
    }
    
    /// <summary>
    /// 触发对话
    /// </summary>
    public void TriggerDialog()
    {
        // 检查是否已经触发过且设置为只触发一次
        if (oneTimeOnly && hasTriggered)
        {
            if (debugMode)
            {
                Debug.Log($"DialogTrigger ({name}): 对话已经触发过，跳过");
            }
            return;
        }
        
        // 检查是否有对话管理器和对话序列
        if (dialogManager == null || dialogToPlay == null)
        {
            if (debugMode)
            {
                Debug.LogWarning($"DialogTrigger ({name}): DialogManager 或 DialogSequence 未设置");
            }
            return;
        }
        
        // 检查是否已经在进行对话
        if (dialogManager.IsDialogActive())
        {
            if (debugMode)
            {
                Debug.Log($"DialogTrigger ({name}): 对话系统正忙，跳过触发");
            }
            return;
        }
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 开始播放对话序列");
        }
        
        // 隐藏交互提示
        ShowInteractPrompt(false);
        
        // 开始对话
        dialogManager.StartDialog(dialogToPlay);
        
        // 标记为已触发
        hasTriggered = true;
    }
    
    /// <summary>
    /// 显示/隐藏交互提示
    /// </summary>
    /// <param name="show">是否显示</param>
    private void ShowInteractPrompt(bool show)
    {
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(show && !hasTriggered);
        }
    }
    
    /// <summary>
    /// 重置触发器（允许再次触发）
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 触发器已重置");
        }
    }
    
    /// <summary>
    /// 设置对话序列
    /// </summary>
    /// <param name="newDialog">新的对话序列</param>
    public void SetDialogSequence(DialogSequence newDialog)
    {
        dialogToPlay = newDialog;
        
        if (debugMode)
        {
            Debug.Log($"DialogTrigger ({name}): 对话序列已更新");
        }
    }
    
    /// <summary>
    /// 获取当前对话序列
    /// </summary>
    /// <returns>当前对话序列</returns>
    public DialogSequence GetDialogSequence()
    {
        return dialogToPlay;
    }
    
    /// <summary>
    /// 检查是否已经触发过
    /// </summary>
    /// <returns>是否已触发</returns>
    public bool HasTriggered()
    {
        return hasTriggered;
    }
    
    #region Gizmos
    
    void OnDrawGizmos()
    {
        // 在Scene视图中显示触发区域
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = hasTriggered ? Color.red : Color.green;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
            
            if (col is BoxCollider)
            {
                BoxCollider boxCol = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphereCol = col as SphereCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphereCol.center, sphereCol.radius);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // 被选中时显示更明显的边框
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.yellow;
            
            if (col is BoxCollider)
            {
                BoxCollider boxCol = col as BoxCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphereCol = col as SphereCollider;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireSphere(sphereCol.center, sphereCol.radius);
            }
        }
    }
    
    #endregion
}
