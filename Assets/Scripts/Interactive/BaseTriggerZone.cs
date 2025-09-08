using UnityEngine;

/// <summary>
/// 触发区域基类，可以被继承来实现不同的触发效果
/// </summary>
public abstract class BaseTriggerZone : MonoBehaviour
{
    [Header("Base Trigger Settings")]
    public bool requiresInput = true;  // 是否需要按键输入
    public KeymapManager.Function triggerKey = KeymapManager.Function.Interact; // 触发按键
    public bool oneTimeOnly = true;   // 是否只能触发一次
    
    protected bool playerInZone = false;
    protected bool isTriggered = false;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            OnPlayerEnterZone(other);
            
            // 如果不需要按键，直接触发
            if (!requiresInput)
            {
                TriggerEffect();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            OnPlayerExitZone(other);
        }
    }

    void Update()
    {
        if (!playerInZone || (oneTimeOnly && isTriggered)) return;

        // 检查按键输入
        if (requiresInput && KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            if (CheckTriggerInput())
            {
                TriggerEffect();
            }
        }
    }

    protected virtual bool CheckTriggerInput()
    {
        return KeymapManager.Singleton.IsKeyPressed(triggerKey);
    }

    protected void TriggerEffect()
    {
        if (oneTimeOnly && isTriggered) return;
        
        isTriggered = true;
        ExecuteEffect();
    }

    // 需要被子类实现的抽象方法
    protected abstract void ExecuteEffect();
    
    // 可以被子类重写的虚方法
    protected virtual void OnPlayerEnterZone(Collider2D player) { }
    protected virtual void OnPlayerExitZone(Collider2D player) { }
}
