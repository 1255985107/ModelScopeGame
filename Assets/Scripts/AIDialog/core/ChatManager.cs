using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 聊天管理器的核心脚本。
/// 这是一个单例，负责协调UI和网络服务之间的所有交互。
/// </summary>
public class ChatManager : MonoBehaviour
{
    // --- 单例模式 ---
    public static ChatManager Instance { get; private set; }
    public static bool IsUIFocused { get; private set; }

    // --- 事件系统 ---
    // UI层将订阅这些事件来响应不同的API结果。这种方式可以实现逻辑和表现的解耦。
    public static event Action OnRequestStarted;
    public static event Action<DialogueResponse> OnDialogueReceived;
    public static event Action<ClarificationResponse> OnClarificationReceived;
    public static event Action<GuidanceResponse> OnGuidanceReceived;
    public static event Action<string> OnRequestFailed;

    // --- 会话状态 ---
    private string _sessionId;
    private string _provider;
    private string _apiKey;
    
    // --- 状态控制 ---
    private bool _isRequestInProgress = false;

    private void Awake()
    {
        Debug.LogWarning("--- ChatManager AWAKE method has been called! ---");
        IsUIFocused = false;
        // 实现单例模式，确保场景中只有一个ChatManager
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // 在游戏开始时生成一个唯一的会话ID
            _sessionId = System.Guid.NewGuid().ToString();
            Debug.Log($"[ChatManager] New session started with ID: {_sessionId}");
        }
    }
    public void SetUIFocus(bool hasFocus)
    {
        IsUIFocused = hasFocus;
    }

    /// <summary>
    /// 由设置UI调用，用于存储用户选择的Provider和他们输入的API Key。
    /// </summary>
    /// <param name="provider">例如 "openai", "deepseek"</param>
    /// <param name="apiKey">用户的API Key</param>
    public void SetCredentials(string provider, string apiKey)
    {
        _provider = provider;
        _apiKey = apiKey;
        Debug.Log($"[ChatManager] Credentials set for provider: {provider}");
    }

    /// <summary>
    /// 由UI的发送按钮或选项按钮调用，这是发起聊天请求的主要入口。
    /// </summary>
    /// <param name="text">用户输入的文本或选择的澄清选项文本</param>
    public async void SendChatMessage(string text)
    {
        // --- 输入验证 ---
        if (_isRequestInProgress)
        {
            Debug.LogWarning("[ChatManager] Request is already in progress. Please wait.");
            return;
        }
        if (string.IsNullOrEmpty(_provider) || string.IsNullOrEmpty(_apiKey))
        {
            Debug.LogError("[ChatManager] Provider or API Key is not set!");
            OnRequestFailed?.Invoke("请先在设置中选择AI服务商并输入API Key。");
            return;
        }
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("[ChatManager] Input text is empty.");
            return;
        }

        // --- 开始处理请求 ---
        _isRequestInProgress = true;
        OnRequestStarted?.Invoke(); // 通知UI层显示“加载中”状态

        // 1. 构建请求数据
        ChatRequest requestData = new ChatRequest
        {
            session_id = _sessionId,
            text = text,
            provider = _provider,
            api_key = _apiKey
        };
        
        // 2. 异步调用ApiService
        try
        {
            APIResponse response = await ApiService.Instance.SendChatRequestAsync(requestData);

            if (response != null)
            {
                // 3. 根据响应类型，调用相应的事件
                // 使用C#的 'is' 模式匹配来安全地检查和转换类型
                if (response is DialogueResponse dialogueResponse)
                {
                    OnDialogueReceived?.Invoke(dialogueResponse);
                }
                else if (response is ClarificationResponse clarificationResponse)
                {
                    OnClarificationReceived?.Invoke(clarificationResponse);
                }
                else if (response is GuidanceResponse guidanceResponse)
                {
                    OnGuidanceReceived?.Invoke(guidanceResponse);
                }
                else
                {
                    throw new Exception($"Unknown response type received: {response.type}");
                }
            }
            else
            {
                // ApiService返回null通常意味着网络层或反序列化层出错
                throw new Exception("Received a null response from ApiService. Check logs for details.");
            }
        }
        catch (Exception e)
        {
            // 4. 捕获任何在请求过程中发生的异常
            Debug.LogError($"[ChatManager] An error occurred while sending chat message: {e.Message}");
            OnRequestFailed?.Invoke("与服务器通信时发生错误，请稍后重试。");
        }
        finally
        {
            // 5. 无论成功与否，最后都将请求状态重置
            _isRequestInProgress = false;
        }
    }
}