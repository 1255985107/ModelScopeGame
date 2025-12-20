using TMPro; // 必须引入TextMeshPro的命名空间
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制所有对话相关UI元素的显示和交互。
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("SWK 显示区")]
    [SerializeField] private GameObject swkPanel; // SWK的整个面板，方便统一显示/隐藏
    [SerializeField] private TextMeshProUGUI swkDialogueText;

    [Header("ADA 显示区")]
    [SerializeField] private GameObject adaPanel; // ADA的整个面板
    [SerializeField] private TextMeshProUGUI adaDialogueText;

    [Header("玩家 (NULL) 输入区")]
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private Button sendButton;

    // --- 生命周期方法 ---

    private void OnEnable()
    {
        // 订阅ChatManager的事件，当事件发生时，调用对应的方法
        ChatManager.OnRequestStarted += HandleRequestStarted;
        ChatManager.OnDialogueReceived += HandleDialogueReceived;
        ChatManager.OnClarificationReceived += HandleClarificationReceived; // 为澄清选项预留
        ChatManager.OnRequestFailed += HandleRequestFailed;

        // 为UI元素添加监听器
        //sendButton.onClick.AddListener(OnSendButtonClicked);
        userInputField.onSelect.AddListener(delegate { OnInputFieldFocusChanged(true); });
        userInputField.onDeselect.AddListener(delegate { OnInputFieldFocusChanged(false); });
    }

    private void OnDisable()
    {
        // 取消订阅，防止在对象销毁后ChatManager还尝试调用它，导致内存泄漏
        ChatManager.OnRequestStarted -= HandleRequestStarted;
        ChatManager.OnDialogueReceived -= HandleDialogueReceived;
        ChatManager.OnClarificationReceived -= HandleClarificationReceived;
        ChatManager.OnRequestFailed -= HandleRequestFailed;

        // 移除UI元素的监听器
        //sendButton.onClick.RemoveAllListeners();
        userInputField.onSelect.RemoveAllListeners();
        userInputField.onDeselect.RemoveAllListeners();
    }

    // --- 事件处理方法 ---

    private void HandleRequestStarted()
    {
        // 当请求开始时，禁用输入框和按钮，防止用户重复发送
        sendButton.interactable = false;
        userInputField.interactable = false;
        // 可以在这里显示一个“对方正在输入...”的动画
    }

    private void HandleDialogueReceived(DialogueResponse response)
    {
        // 清空上一轮的对话内容
        swkDialogueText.text = "...";
        adaDialogueText.text = "...";

        // 遍历后端返回的对话数据并更新UI
        foreach (var item in response.data.responses)
        {
            if (item.character == "SWK")
            {
                swkDialogueText.text = item.text;
            }
            else if (item.character == "ADA")
            {
                adaDialogueText.text = item.text;
            }
        }
        
        // 恢复UI交互
        ResetInputUI();
    }

    private void HandleClarificationReceived(ClarificationResponse response)
    {
        // TODO: 在这里处理澄清选项的显示逻辑
        Debug.Log("收到了澄清选项，需要UI显示它们。");

        // 同样需要恢复UI交互
        ResetInputUI();
    }
    
    private void HandleRequestFailed(string errorMessage)
    {
        // 当请求失败时，可以在某个UI元素上显示错误信息
        // 这里我们暂时只在ADA的对话框显示错误
        adaDialogueText.text = $"<color=red>错误: {errorMessage}</color>";
        
        // 恢复UI交互
        ResetInputUI();
    }


    // --- UI交互方法 ---

    /// <summary>
    /// 当“发送”按钮被点击时调用。
    /// </summary>
    public void OnSendButtonClicked()
    {
        string inputText = userInputField.text;
        
        // 检查输入是否为空
        if (!string.IsNullOrWhiteSpace(inputText))
        {
            // 通过ChatManager单例发送消息
            ChatManager.Instance.SendChatMessage(inputText);
            
            // 清空输入框
            userInputField.text = "";
        }
    }
    
    /// <summary>
    /// 当输入框被选中或取消选中时调用。
    /// </summary>
    private void OnInputFieldFocusChanged(bool isFocused)
    {
        // 通知ChatManager UI焦点状态已改变
        ChatManager.Instance.SetUIFocus(isFocused);
        Debug.Log($"UI 焦点状态: {isFocused}");
    }

    /// <summary>
    /// 一个辅助方法，用于将输入UI重置为可交互状态。
    /// </summary>
    private void ResetInputUI()
    {
        sendButton.interactable = true;
        userInputField.interactable = true;
        userInputField.Select(); // 自动重新选中输入框，方便连续输入
        userInputField.ActivateInputField();
    }
}