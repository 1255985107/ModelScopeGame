using System;
using UnityEngine;

/// <summary>
/// 对话数据结构
/// </summary>
[Serializable]
public class DialogData
{
    [Header("对话内容")]
    public string characterName;        // 角色名
    public string dialogText;           // 对话文本
    public AudioClip voiceClip;         // 语音剪辑
    
    [Header("显示设置")]
    public float textDisplaySpeed = 0.05f;  // 文字显示速度（每个字符间隔时间）
    public bool waitForInput = true;        // 是否等待玩家输入继续
    public float autoAdvanceDelay = 3f;     // 自动继续的延迟时间（如果不等待输入）
    
    [Header("可选设置")]
    public Sprite characterPortrait;    // 角色头像
    public Color textColor = Color.white;   // 文字颜色
}
