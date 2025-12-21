using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这是为了在Unity编辑器中方便地设置后端地址
[CreateAssetMenu(fileName = "ApiConfig", menuName = "Config/API Configuration")]
public class ApiConfig : ScriptableObject
{
    public string baseUri = "http://127.0.0.1:8000";
    public string chatEndpoint = "/chat/send";
}