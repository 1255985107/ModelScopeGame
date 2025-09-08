using UnityEngine;

/// <summary>
/// 教程触发区域，用于游戏关卡中的各种教程和事件触发
/// </summary>
public class TutorialTriggerZone : BaseTriggerZone
{
    [Header("Tutorial Settings")]
    public LevelManager levelManager;
    public TutorialType tutorialType = TutorialType.Walk;
    
    public enum TutorialType
    {
        Walk,      // 行走教程
        Jump,      // 跳跃教程
        Final,     // 最终触发
        Interact   // 交互教程
    }

    protected override void OnPlayerEnterZone(Collider2D player)
    {
        if (levelManager == null) return;

        // 根据教程类型设置不同的输入要求
        switch (tutorialType)
        {
            case TutorialType.Walk:
                requiresInput = true;
                break;
            case TutorialType.Jump:
                requiresInput = true;
                break;
            case TutorialType.Final:
                requiresInput = false; // 最终触发不需要按键
                break;
            case TutorialType.Interact:
                requiresInput = true;
                triggerKey = KeymapManager.Function.Interact;
                break;
        }
    }

    protected override bool CheckTriggerInput()
    {
        if (KeymapManager.Singleton == null || !KeymapManager.Singleton.IsReady)
            return false;

        switch (tutorialType)
        {
            case TutorialType.Walk:
                return KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft) || 
                       KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight);
            case TutorialType.Jump:
                return KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp);
            case TutorialType.Interact:
                return KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.Interact);
            default:
                return base.CheckTriggerInput();
        }
    }

    protected override void ExecuteEffect()
    {
        if (levelManager == null) return;

        switch (tutorialType)
        {
            case TutorialType.Walk:
                levelManager.OnlyWalkTrigger();
                levelManager.OnTriggerZoneActivated();
                break;
            case TutorialType.Jump:
                levelManager.JumpTutorialTrigger();
                levelManager.OnTriggerZoneActivated();
                break;
            case TutorialType.Final:
                levelManager.FinalFallTrigger();
                break;
            case TutorialType.Interact:
                levelManager.OnTriggerZoneActivated();
                break;
        }
        
        Debug.Log($"Tutorial trigger executed: {tutorialType}");
    }
}
