using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 通用触发区域，可以通过Unity Events配置任意效果
/// </summary>
public class GenericTriggerZone : BaseTriggerZone
{
    [Header("Generic Trigger Events")]
    public UnityEvent onTriggerEnter;     // 玩家进入时触发
    public UnityEvent onTriggerActivate;  // 按键触发时执行
    public UnityEvent onTriggerExit;      // 玩家离开时触发

    protected override void OnPlayerEnterZone(Collider2D player)
    {
        onTriggerEnter?.Invoke();
    }

    protected override void OnPlayerExitZone(Collider2D player)
    {
        onTriggerExit?.Invoke();
    }

    protected override void ExecuteEffect()
    {
        onTriggerActivate?.Invoke();
    }
}
