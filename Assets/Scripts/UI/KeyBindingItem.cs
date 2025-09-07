using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// 单个按键绑定UI项目，负责显示和修改一个功能的按键绑定
/// </summary>
public class KeyBindingItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI functionNameText;
    [SerializeField] private Button keyButton;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private GameObject waitingForInputIndicator;
    
    private KeymapManager.Function assignedFunction;
    private bool isWaitingForInput = false;
    private bool isInitialized = false;
    
    void Awake()
    {
        // 如果UI引用为空，尝试自动查找
        Debug.Assert(functionNameText != null, "FunctionName TextMeshProUGUI is not assigned in the inspector.");
        Debug.Assert(keyButton != null, "Key Button is not assigned in the inspector.");
        Debug.Assert(keyText != null, "Key TextMeshProUGUI is not assigned in the inspector.");

        keyButton.onClick.AddListener(OnKeyButtonClicked);
        
        if (waitingForInputIndicator == null)
            waitingForInputIndicator = transform.Find("WaitingIndicator")?.gameObject;
    }
    
    /// <summary>
    /// 初始化按键绑定项目
    /// </summary>
    /// <param name="function">要绑定的功能</param>
    public void Initialize(KeymapManager.Function function)
    {
        assignedFunction = function;
        isInitialized = true;
        RefreshDisplay();
    }
    
    /// <summary>
    /// 刷新显示
    /// </summary>
    public void RefreshDisplay()
    {
        if (!isInitialized || KeymapManager.Singleton == null) return;
        
        // 更新功能名称
        if (functionNameText != null)
        {
            functionNameText.text = KeymapManager.Singleton.GetFunctionDisplayName(assignedFunction);
        }
        
        // 更新按键显示
        UpdateKeyDisplay();
    }
    
    void UpdateKeyDisplay()
    {
        if (keyText == null || KeymapManager.Singleton == null) return;
        
        Key currentKey = KeymapManager.Singleton.GetKey(assignedFunction);
        string keyDisplayName = GetKeyDisplayName(currentKey);
        
        keyText.text = keyDisplayName;
    }
    
    /// <summary>
    /// 获取按键的显示名称
    /// </summary>
    /// <param name="key">按键</param>
    /// <returns>显示名称</returns>
    string GetKeyDisplayName(Key key)
    {
        switch (key)
        {
            case Key.None: return "None";
            case Key.Space: return "Space";
            case Key.Enter: return "Enter";
            case Key.Tab: return "Tab";
            case Key.Backspace: return "Backspace";
            case Key.Delete: return "Delete";
            case Key.Escape: return "Escape";
            case Key.LeftShift: return "L Shift";
            case Key.RightShift: return "R Shift";
            case Key.LeftCtrl: return "L Ctrl";
            case Key.RightCtrl: return "R Ctrl";
            case Key.LeftAlt: return "L Alt";
            case Key.RightAlt: return "R Alt";
            case Key.UpArrow: return "↑";
            case Key.DownArrow: return "↓";
            case Key.LeftArrow: return "←";
            case Key.RightArrow: return "→";
            case Key.Digit0: return "0";
            case Key.Digit1: return "1";
            case Key.Digit2: return "2";
            case Key.Digit3: return "3";
            case Key.Digit4: return "4";
            case Key.Digit5: return "5";
            case Key.Digit6: return "6";
            case Key.Digit7: return "7";
            case Key.Digit8: return "8";
            case Key.Digit9: return "9";
            default:
                return key.ToString().ToUpper();
        }
    }
    
    /// <summary>
    /// 按键按钮点击事件
    /// </summary>
    void OnKeyButtonClicked()
    {
        if (isWaitingForInput) return;
        
        StartKeyRebinding();
    }
    
    /// <summary>
    /// 开始按键重绑定
    /// </summary>
    void StartKeyRebinding()
    {
        isWaitingForInput = true;
        
        // 显示等待输入指示器
        if (waitingForInputIndicator != null)
            waitingForInputIndicator.SetActive(true);
        
        // 更新按钮文本
        if (keyText != null)
            keyText.text = "Press a key...";
        
        // 禁用按钮
        if (keyButton != null)
            keyButton.interactable = false;
        
        Debug.Log($"Waiting for key input for function: {assignedFunction}");
    }
    
    /// <summary>
    /// 停止按键重绑定
    /// </summary>
    void StopKeyRebinding()
    {
        isWaitingForInput = false;
        
        // 隐藏等待输入指示器
        if (waitingForInputIndicator != null)
            waitingForInputIndicator.SetActive(false);
        
        // 启用按钮
        if (keyButton != null)
            keyButton.interactable = true;
        
        // 刷新显示
        RefreshDisplay();
    }
    
    void Update()
    {
        if (!isWaitingForInput || !isInitialized) return;
        
        // 检查Escape键取消重绑定
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            StopKeyRebinding();
            Debug.Log("Key rebinding cancelled");
            return;
        }
        
        // 检查所有有效的按键输入
        Key[] validKeys = GetValidKeys();
        foreach (Key key in validKeys)
        {
            try
            {
                if (Keyboard.current[key].wasPressedThisFrame)
                {
                    TrySetNewKey(key);
                    return;
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                // 忽略无效的键值，继续检查下一个
                continue;
            }
        }
    }
    
    /// <summary>
    /// 获取所有有效的按键列表
    /// </summary>
    /// <returns>有效按键数组</returns>
    Key[] GetValidKeys()
    {
        return new Key[]
        {
            // 字母键
            Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J,
            Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T,
            Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
            
            // 数字键
            Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
            Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9,
            
            // 功能键
            Key.F1, Key.F2, Key.F3, Key.F4, Key.F5, Key.F6,
            Key.F7, Key.F8, Key.F9, Key.F10, Key.F11, Key.F12,
            
            // 方向键
            Key.UpArrow, Key.DownArrow, Key.LeftArrow, Key.RightArrow,
            
            // 修饰键
            Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl,
            Key.LeftAlt, Key.RightAlt,
            
            // 常用特殊键
            Key.Space, Key.Enter, Key.Tab, Key.Backspace, Key.Delete,
            Key.Insert, Key.Home, Key.End, Key.PageUp, Key.PageDown,
            
            // 标点符号键
            Key.Comma, Key.Period, Key.Slash, Key.Semicolon, Key.Quote,
            Key.LeftBracket, Key.RightBracket, Key.Backslash, Key.Minus, Key.Equals,
            Key.Backquote,
            
            // 小键盘
            Key.Numpad0, Key.Numpad1, Key.Numpad2, Key.Numpad3, Key.Numpad4,
            Key.Numpad5, Key.Numpad6, Key.Numpad7, Key.Numpad8, Key.Numpad9,
            Key.NumpadDivide, Key.NumpadMultiply, Key.NumpadMinus, Key.NumpadPlus,
            Key.NumpadEnter, Key.NumpadPeriod
        };
    }

    /// <summary>
    /// 尝试设置新按键
    /// </summary>
    /// <param name="newKey">新按键</param>
    void TrySetNewKey(Key newKey)
    {
        if (KeymapManager.Singleton == null)
        {
            StopKeyRebinding();
            return;
        }
        
        // 检查是否与其他功能冲突
        KeymapManager.Function? conflictingFunction = KeymapManager.Singleton.GetConflictingFunction(newKey, assignedFunction);
        
        if (conflictingFunction.HasValue)
        {
            Debug.LogWarning($"Key {newKey} is already bound to {conflictingFunction.Value}");
            
            // 可以在这里显示冲突对话框
            ShowConflictDialog(newKey, conflictingFunction.Value);
            return;
        }
        
        // 设置新按键
        bool success = KeymapManager.Singleton.SetKey(assignedFunction, newKey);
        
        if (success)
        {
            Debug.Log($"Successfully bound {assignedFunction} to {newKey}");
            StopKeyRebinding();
        }
        else
        {
            Debug.LogError($"Failed to bind {assignedFunction} to {newKey}");
            StopKeyRebinding();
        }
    }
    
    /// <summary>
    /// 显示按键冲突对话框
    /// </summary>
    /// <param name="newKey">新按键</param>
    /// <param name="conflictingFunction">冲突的功能</param>
    void ShowConflictDialog(Key newKey, KeymapManager.Function conflictingFunction)
    {
        // 这里可以实现一个对话框询问是否要覆盖现有绑定
        // 暂时直接取消重绑定
        Debug.LogWarning($"Key conflict detected: {newKey} is already bound to {conflictingFunction}. Rebinding cancelled.");
        StopKeyRebinding();
        
        // TODO: 实现冲突对话框UI
        // 可以显示一个弹窗询问用户是否要：
        // 1. 取消重绑定
        // 2. 覆盖现有绑定
        // 3. 交换两个功能的按键
    }
    
    /// <summary>
    /// 强制设置按键（忽略冲突）
    /// </summary>
    /// <param name="newKey">新按键</param>
    public void ForceSetKey(Key newKey)
    {
        if (KeymapManager.Singleton != null)
        {
            KeymapManager.Singleton.SetKey(assignedFunction, newKey);
            RefreshDisplay();
        }
    }
    
    /// <summary>
    /// 获取当前绑定的按键
    /// </summary>
    /// <returns>当前按键</returns>
    public Key GetCurrentKey()
    {
        if (KeymapManager.Singleton != null)
        {
            return KeymapManager.Singleton.GetKey(assignedFunction);
        }
        return Key.None;
    }
    
    /// <summary>
    /// 获取分配的功能
    /// </summary>
    /// <returns>功能枚举</returns>
    public KeymapManager.Function GetAssignedFunction()
    {
        return assignedFunction;
    }
    
    void OnDestroy()
    {
        // 如果正在等待输入，停止重绑定
        if (isWaitingForInput)
        {
            StopKeyRebinding();
        }
    }
}
