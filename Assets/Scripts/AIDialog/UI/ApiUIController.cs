using TMPro;
using UnityEngine;

/// <summary>
/// 负责处理AI凭证输入的UI控制器。
/// 它的唯一职责是监听UI控件的变化，并将数据自动保存到ChatManager。
/// </summary>
public class SettingsUIController : MonoBehaviour
{
    [Header("UI 引用")]
    [Tooltip("包含服务商选项的下拉菜单")]
    [SerializeField] private TMP_Dropdown providerDropdown;

    [Tooltip("用于输入API Key的输入框")]
    [SerializeField] private TMP_InputField apiKeyInputField;

    // 用于在内部暂存数据，以避免每次输入都读取UI控件
    private string _currentProvider;
    private string _currentApiKey;

    private void Start()
    {
        Debug.LogWarning("--- SettingsUIController START method has been called! ---"); 
        // --- 核心逻辑：在脚本启动时，立即订阅UI控件的“值改变”事件 ---

        // 1. 订阅下拉菜单的 onValueChanged 事件
        // 当用户选择一个新的服务商时，OnProviderChanged 方法将被调用
        providerDropdown.onValueChanged.AddListener(OnProviderChanged);

        // 2. 订阅输入框的 onValueChanged 事件
        // 当用户在输入框里打字时，OnApiKeyChanged 方法将被调用
        apiKeyInputField.onValueChanged.AddListener(OnApiKeyChanged);

        // --- 初始化 ---
        // 立即读取一次下拉菜单的初始值，确保在用户从未更改过选项的情况下，provider也有值
        OnProviderChanged(providerDropdown.value);
    }

    private void OnDestroy()
    {
        // 在对象销毁时移除监听器，这是一个健壮的编程习惯
        if (providerDropdown != null)
        {
            providerDropdown.onValueChanged.RemoveAllListeners();
        }
        if (apiKeyInputField != null)
        {
            apiKeyInputField.onValueChanged.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 当下拉菜单的值发生变化时被调用。
    /// </summary>
    private void OnProviderChanged(int index)
    {
        // 从下拉菜单的选项列表中获取新选择的文本
        _currentProvider = providerDropdown.options[index].text;
        
        // 调用核心保存方法
        TryAutoSaveCredentials();
    }

    /// <summary>
    /// 当API Key输入框的内容发生变化时被调用。
    /// </summary>
    private void OnApiKeyChanged(string newText)
    {
        // 更新内部存储的API Key
        _currentApiKey = newText;

        // 调用核心保存方法
        TryAutoSaveCredentials();
    }

    /// <summary>
    /// 尝试自动保存凭证的核心方法。
    /// </summary>
    private void TryAutoSaveCredentials()
    {
        // 只有当两个字段都有内容时，才进行保存
        if (!string.IsNullOrEmpty(_currentProvider) && !string.IsNullOrWhiteSpace(_currentApiKey))
        {
            // 通过ChatManager单例，立即更新全局凭证
            // 这是与您现有系统集成的关键
            ChatManager.Instance.SetCredentials(_currentProvider, _currentApiKey);
            
            // 在控制台打印日志，方便调试，确认保存已发生
            Debug.Log($"[Settings] 凭证已自动更新到 ChatManager: Provider={_currentProvider}, API Key=******");
        }
    }
}