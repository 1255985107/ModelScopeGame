using UnityEngine;

/// <summary>
/// 对话序列配置
/// </summary>
[CreateAssetMenu(fileName = "DialogSequence", menuName = "Dialog/DialogSequence")]
public class DialogSequence : ScriptableObject
{
    [Header("对话序列")]
    public DialogData[] dialogs;
    
    [Header("序列设置")]
    public bool loopSequence = false;   // 是否循环播放
    public bool pauseGameDuringDialog = true; // 对话期间是否暂停游戏
}
