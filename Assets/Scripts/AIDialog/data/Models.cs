using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// 这个文件包含了所有与后端API交互所需的数据模型。
/// 它们是前后端通信的“合同”。
/// </summary>

// --- 请求模型 ---

[System.Serializable]
public class ChatRequest
{
    [JsonProperty("session_id")]
    public string session_id;
    
    [JsonProperty("text")]
    public string text;
    
    [JsonProperty("provider")]
    public string provider;
    
    [JsonProperty("api_key")]
    public string api_key;
}


// --- 响应模型 ---

/// <summary>
/// 所有API响应的基类。
/// 我们使用这个基类和继承模式来处理后端可能返回的多种不同结构的响应。
/// ApiService中的自定义JsonConverter会根据'type'字段来决定实例化哪个子类。
/// </summary>
[System.Serializable]
public class APIResponse
{
    public string type;
}

// --- 具体的响应类型 ---

// 用于 type = "dialogue"
[System.Serializable]
public class DialogueItem
{
    public string character;
    public string text;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueItem> responses;
}

[System.Serializable]
public class DialogueResponse : APIResponse
{
    public DialogueData data;
}


// 用于 type = "clarification"
[System.Serializable]
public class ClarificationResponse : APIResponse
{
    public List<string> data;
}


// 用于 type = "guidance"
[System.Serializable]
public class GuidanceData
{
    public string text;
}

[System.Serializable]
public class GuidanceResponse : APIResponse
{
    public GuidanceData data;
}