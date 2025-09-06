using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeymapManager : MonoBehaviour
{
    public static KeymapManager Singleton { get; private set; }

    public enum Function
    {
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown,
        Interact,
        ExitMenu
    }

    [Header("Input Actions")]
    public InputActionAsset inputActions;
    
    // 添加IsReady属性
    public bool IsReady { get; private set; } = false;

    private Dictionary<Function, InputAction> functionToAction = new Dictionary<Function, InputAction>();
    private Dictionary<Function, Key> defaultKeys = new Dictionary<Function, Key>();

    // 默认按键配置
    private readonly Dictionary<Function, Key> defaultKeyBindings = new Dictionary<Function, Key>
    {
        { Function.MoveUp, Key.W },
        { Function.MoveDown, Key.S },
        { Function.MoveLeft, Key.A },
        { Function.MoveRight, Key.D },
        { Function.Interact, Key.E },
        { Function.ExitMenu, Key.Escape }
    };

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            InitializeInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeInputActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return;
        }

        // 清空现有映射
        functionToAction.Clear();
        defaultKeys.Clear();
        
        // 初始化功能到动作的映射
        foreach (Function function in Enum.GetValues(typeof(Function)))
        {
            string actionName = function.ToString();
            InputAction action = inputActions.FindAction(actionName);
            
            if (action != null)
            {
                functionToAction[function] = action;
                defaultKeys[function] = defaultKeyBindings[function];
                Debug.Log($"Mapped function {function} to action {actionName}");
            }
            else
            {
                Debug.LogWarning($"Input Action '{actionName}' not found in InputActionAsset!");
            }
        }

        // 启用输入动作
        inputActions.Enable();
        
        // 标记为就绪
        IsReady = true;
        Debug.Log("KeyRebindManager initialized and ready");
    }

    /// <summary>
    /// 获取指定功能的当前按键
    /// </summary>
    public Key GetKey(Function function)
    {
        if (functionToAction.TryGetValue(function, out InputAction action) && action.bindings.Count > 0)
        {
            var binding = action.bindings[0];
            string effectivePath = binding.effectivePath;
            
            if (!string.IsNullOrEmpty(effectivePath))
            {
                // 处理键盘按键
                if (effectivePath.StartsWith("<Keyboard>/"))
                {
                    string keyString = effectivePath.Replace("<Keyboard>/", "");
                    
                    // 处理特殊按键映射
                    switch (keyString.ToLower())
                    {
                        case "1": return Key.Digit1;
                        case "2": return Key.Digit2;
                        case "3": return Key.Digit3;
                        case "4": return Key.Digit4;
                        case "5": return Key.Digit5;
                        case "6": return Key.Digit6;
                        case "7": return Key.Digit7;
                        case "8": return Key.Digit8;
                        case "9": return Key.Digit9;
                        case "0": return Key.Digit0;
                        case "leftshift": return Key.LeftShift;
                        case "rightshift": return Key.RightShift;
                        case "leftctrl": return Key.LeftCtrl;
                        case "rightctrl": return Key.RightCtrl;
                        case "leftalt": return Key.LeftAlt;
                        case "rightalt": return Key.RightAlt;
                        case "space": return Key.Space;
                        case "escape": return Key.Escape;
                        case "enter": return Key.Enter;
                        case "tab": return Key.Tab;
                        case "backspace": return Key.Backspace;
                        case "delete": return Key.Delete;
                        case "uparrow": return Key.UpArrow;
                        case "downarrow": return Key.DownArrow;
                        case "leftarrow": return Key.LeftArrow;
                        case "rightarrow": return Key.RightArrow;
                        default:
                            // 尝试直接解析字母按键
                            if (keyString.Length == 1 && char.IsLetter(keyString[0]))
                            {
                                if (Enum.TryParse<Key>(keyString.ToUpper(), out Key letterKey))
                                {
                                    return letterKey;
                                }
                            }
                            
                            // 尝试直接解析其他按键
                            if (Enum.TryParse<Key>(keyString, true, out Key parsedKey))
                            {
                                return parsedKey;
                            }
                            break;
                    }
                }
                // 处理鼠标按键
                else if (effectivePath.StartsWith("<Mouse>/"))
                {
                    string mouseButton = effectivePath.Replace("<Mouse>/", "");
                    switch (mouseButton)
                    {
                        case "leftButton": return Key.None; // 可以扩展支持鼠标
                        case "rightButton": return Key.None;
                        case "middleButton": return Key.None;
                    }
                }
            }
        }
        
        // 如果无法从InputAction获取，返回默认按键
        return defaultKeyBindings.GetValueOrDefault(function, Key.None);
    }

    /// <summary>
    /// 设置指定功能的按键
    /// </summary>
    public bool SetKey(Function function, Key newKey)
    {
        // 检查冲突
        if (HasKeyConflict(newKey, function))
        {
            Debug.LogWarning($"Key {newKey} is already bound to another function!");
            return false;
        }

        if (functionToAction.TryGetValue(function, out InputAction action))
        {
            // 获取正确的按键路径
            string keyPath = GetKeyPath(newKey);
            if (!string.IsNullOrEmpty(keyPath))
            {
                action.ApplyBindingOverride(0, keyPath);
                Debug.Log($"Successfully bound {function} to {newKey} (path: {keyPath})");
                return true;
            }
            else
            {
                Debug.LogError($"Unable to get key path for {newKey}");
                return false;
            }
        }

        Debug.LogError($"Function {function} not found in functionToAction dictionary!");
        return false;
    }
    
    /// <summary>
    /// 获取按键的输入路径
    /// </summary>
    private string GetKeyPath(Key key)
    {
        switch (key)
        {
            case Key.Digit1: return "<Keyboard>/1";
            case Key.Digit2: return "<Keyboard>/2";
            case Key.Digit3: return "<Keyboard>/3";
            case Key.Digit4: return "<Keyboard>/4";
            case Key.Digit5: return "<Keyboard>/5";
            case Key.Digit6: return "<Keyboard>/6";
            case Key.Digit7: return "<Keyboard>/7";
            case Key.Digit8: return "<Keyboard>/8";
            case Key.Digit9: return "<Keyboard>/9";
            case Key.Digit0: return "<Keyboard>/0";
            case Key.LeftShift: return "<Keyboard>/leftShift";
            case Key.RightShift: return "<Keyboard>/rightShift";
            case Key.LeftCtrl: return "<Keyboard>/leftCtrl";
            case Key.RightCtrl: return "<Keyboard>/rightCtrl";
            case Key.LeftAlt: return "<Keyboard>/leftAlt";
            case Key.RightAlt: return "<Keyboard>/rightAlt";
            case Key.Space: return "<Keyboard>/space";
            case Key.Escape: return "<Keyboard>/escape";
            case Key.Enter: return "<Keyboard>/enter";
            case Key.Tab: return "<Keyboard>/tab";
            case Key.Backspace: return "<Keyboard>/backspace";
            case Key.Delete: return "<Keyboard>/delete";
            case Key.UpArrow: return "<Keyboard>/upArrow";
            case Key.DownArrow: return "<Keyboard>/downArrow";
            case Key.LeftArrow: return "<Keyboard>/leftArrow";
            case Key.RightArrow: return "<Keyboard>/rightArrow";
            // 字母按键
            case Key.A: return "<Keyboard>/a";
            case Key.B: return "<Keyboard>/b";
            case Key.C: return "<Keyboard>/c";
            case Key.D: return "<Keyboard>/d";
            case Key.E: return "<Keyboard>/e";
            case Key.F: return "<Keyboard>/f";
            case Key.G: return "<Keyboard>/g";
            case Key.H: return "<Keyboard>/h";
            case Key.I: return "<Keyboard>/i";
            case Key.J: return "<Keyboard>/j";
            case Key.K: return "<Keyboard>/k";
            case Key.L: return "<Keyboard>/l";
            case Key.M: return "<Keyboard>/m";
            case Key.N: return "<Keyboard>/n";
            case Key.O: return "<Keyboard>/o";
            case Key.P: return "<Keyboard>/p";
            case Key.Q: return "<Keyboard>/q";
            case Key.R: return "<Keyboard>/r";
            case Key.S: return "<Keyboard>/s";
            case Key.T: return "<Keyboard>/t";
            case Key.U: return "<Keyboard>/u";
            case Key.V: return "<Keyboard>/v";
            case Key.W: return "<Keyboard>/w";
            case Key.X: return "<Keyboard>/x";
            case Key.Y: return "<Keyboard>/y";
            case Key.Z: return "<Keyboard>/z";
            default: 
                // 对于其他按键，尝试使用小写转换
                return $"<Keyboard>/{key.ToString().ToLower()}";
        }
    }

    /// <summary>
    /// 检查按键是否与其他功能冲突
    /// </summary>
    public bool HasKeyConflict(Key key, Function excludeFunction = Function.MoveUp)
    {
        foreach (var kvp in functionToAction)
        {
            if (kvp.Key == excludeFunction) continue;
            
            if (GetKey(kvp.Key) == key)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取与指定按键冲突的功能
    /// </summary>
    public Function? GetConflictingFunction(Key key, Function excludeFunction = Function.MoveUp)
    {
        foreach (var kvp in functionToAction)
        {
            if (kvp.Key == excludeFunction) continue;
            
            if (GetKey(kvp.Key) == key)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    /// <summary>
    /// 重置所有按键为默认配置
    /// </summary>
    public void ResetKeys()
    {
        foreach (var kvp in defaultKeyBindings)
        {
            if (functionToAction.TryGetValue(kvp.Key, out InputAction action))
            {
                // 使用GetKeyPath获取正确的路径
                string keyPath = GetKeyPath(kvp.Value);
                action.ApplyBindingOverride(0, keyPath);
                Debug.Log($"Reset {kvp.Key} to {kvp.Value} (path: {keyPath})");
            }
        }
        
        Debug.Log("All keys reset to default configuration");
    }

    /// <summary>
    /// 检查指定功能的按键是否被按下
    /// </summary>
    public bool IsKeyPressed(Function function)
    {
        if (!IsReady || inputActions == null || !inputActions.enabled)
        {
            return false;
        }
        
        if (functionToAction.TryGetValue(function, out InputAction action))
        {
            return action.WasPressedThisFrame();
        }
        return false;
    }

    /// <summary>
    /// 检查指定功能的按键是否被持续按住
    /// </summary>
    public bool IsKeyHeld(Function function)
    {
        if (!IsReady || inputActions == null || !inputActions.enabled)
        {
            return false;
        }
        
        if (functionToAction.TryGetValue(function, out InputAction action))
        {
            return action.IsPressed();
        }
        return false;
    }

    /// <summary>
    /// 检查指定功能的按键是否被释放
    /// </summary>
    public bool IsKeyReleased(Function function)
    {
        if (!IsReady || inputActions == null || !inputActions.enabled)
        {
            return false;
        }
        
        if (functionToAction.TryGetValue(function, out InputAction action))
        {
            return action.WasReleasedThisFrame();
        }
        return false;
    }

    /// <summary>
    /// 获取所有功能列表
    /// </summary>
    public Function[] GetAllFunctions()
    {
        return (Function[])Enum.GetValues(typeof(Function));
    }

    /// <summary>
    /// 获取功能的显示名称
    /// </summary>
    public string GetFunctionDisplayName(Function function)
    {
        switch (function)
        {
            case Function.MoveUp: return "Move Up";
            case Function.MoveDown: return "Move Down";
            case Function.MoveLeft: return "Move Left";
            case Function.MoveRight: return "Move Right";
            case Function.ExitMenu: return "Exit Menu";
            default: return function.ToString();
        }
    }

    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
        }
    }
}
