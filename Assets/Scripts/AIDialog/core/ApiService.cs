using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // 引入Newtonsoft.Json命名空间

// 这是为了在Unity编辑器中方便地设置后端地址
[CreateAssetMenu(fileName = "ApiConfig", menuName = "Config/API Configuration")]
public class ApiConfig : ScriptableObject
{
    public string baseUri = "http://127.0.0.1:8000";
    public string chatEndpoint = "/chat/send";
}

public class ApiService : MonoBehaviour
{
    // 在Unity编辑器中拖拽你的ApiConfig文件到这里
    [SerializeField] private ApiConfig apiConfig;
    
    // --- 单例模式 ---
    // 这使得游戏中的任何脚本都可以通过 ApiService.Instance 访问这个服务
    private static ApiService _instance;
    public static ApiService Instance
    {
        get
        {
            if (_instance == null)
            {
                // 如果场景中没有实例，就动态创建一个
                GameObject go = new GameObject("ApiService");
                _instance = go.AddComponent<ApiService>();
                // 你也可以在这里加载ApiConfig，或者确保它被手动设置
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 确保场景中只有一个ApiService实例
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            // 保持这个对象在切换场景时不被销毁
            DontDestroyOnLoad(this.gameObject); 
        }
    }

    /// <summary>
    /// 异步发送聊天请求到后端API。
    /// 这是与后端通信的核心方法。
    /// </summary>
    /// <param name="requestData">包含会话ID、文本、provider和api_key的请求对象</param>
    /// <returns>一个包含API响应或null（如果出错）的Task</returns>
    public async Task<APIResponse> SendChatRequestAsync(ChatRequest requestData)
    {
        // 1. 构建完整的URL
        string uri = apiConfig.baseUri + apiConfig.chatEndpoint;
        
        // 2. 将C#请求对象序列化成JSON字符串
        string jsonPayload = JsonConvert.SerializeObject(requestData);
        
        // 3. 将JSON字符串转换成UTF8编码的字节数组
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // 4. 创建UnityWebRequest对象
        using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
        {
            // 5. 设置请求体和请求头
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[ApiService] 发送请求到 {uri} with payload: {jsonPayload}");

            // 6. 发送请求并异步等待响应
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield(); // 释放控制权，防止游戏卡死
            }

            // 7. 处理响应结果
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log($"[ApiService] 收到成功响应: {jsonResponse}");

                // 8. 将JSON响应反序列化成C#的APIResponse对象
                try
                {
                    // 使用Json.NET的强大功能来处理Union/OneOf类型的反序列化
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(jsonResponse, new APIResponseConverter());
                    return response;
                }
                catch (JsonException ex)
                {
                    Debug.LogError($"[ApiService] JSON反序列化失败: {ex.Message}");
                    return null;
                }
            }
            else
            {
                // 处理网络错误或后端返回的HTTP错误 (如 401, 500)
                Debug.LogError($"[ApiService] 请求失败: {request.error}");
                Debug.LogError($"[ApiService] 错误详情: {request.downloadHandler.text}");
                // 你可以在这里解析错误详情，并将其传递给上层逻辑
                return null;
            }
        }
    }
}


// --- Newtonsoft.Json的自定义转换器 ---
// 由于我们的APIResponse是一个联合类型(Union)，我们需要一个自定义转换器来帮助Json.NET正确地反序列化它。
public class APIResponseConverter : JsonConverter
{
    public override bool CanConvert(System.Type objectType)
    {
        return objectType == typeof(APIResponse);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
    {
        // 将JSON加载到一个临时的JObject中，以便我们可以检查'type'字段
        var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);
        var type = (string)jObject["type"];

        // 根据'type'字段的值，决定要反序列化成哪个具体的C#类
        switch (type)
        {
            case "dialogue":
                return jObject.ToObject<DialogueResponse>(serializer);
            case "clarification":
                return jObject.ToObject<ClarificationResponse>(serializer);
            case "guidance":
                return jObject.ToObject<GuidanceResponse>(serializer);
            default:
                throw new JsonSerializationException($"未知的API响应类型: {type}");
        }
    }

    // 我们只需要处理读取JSON（反序列化），不需要写入JSON（序列化）
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new System.NotImplementedException();
    }
}