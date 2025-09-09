using UnityEngine;
using System; // 必须引用，用于 Action 事件

public class PlayerHealth : MonoBehaviour
{
    [Header("基础设置")]
    public float maxHealth = 12f;   // 最大血量
    public float currentHealth;     // 当前血量

    [Header("怪物/战斗设置")]
    public float damageFromMonster = 1.5f; // 被怪物碰到扣多少血
    public float invincibilityTime = 1.0f; // 受伤后的无敌时间(秒)
    private float lastDamageTime = -10f;   // 上次受伤时刻

    // 定义事件：通知 UI 更新 (参数: 当前血量, 最大血量)
    public static event Action<float, float> onHealthChanged;

    private void Start()
    {
        // 初始化血量
        currentHealth = maxHealth;
        Debug.Log("玩家血量初始化为 " + currentHealth);
        // 游戏开始时，强制刷新一次 UI
        UpdateUI();
    }

    // =========================================================
    // 区域 A: 碰撞检测 (自动扣血逻辑)
    // =========================================================
    
    // 情况1：怪物有实体碰撞体 (BoxCollider2D 等)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("PlayerHealth.OnCollisionEnter2D() Hit Tag: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Monster"))
        {
            TryTakeDamage(damageFromMonster);
        }
    }

    // 情况2：怪物是触发器 (Is Trigger 勾选)
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("PlayerHealth.OnTriggerEnter2D() Hit Tag: " + other.tag);
        if (other.CompareTag("Monster"))
        {
            TryTakeDamage(damageFromMonster);
        }
    }

    // 尝试扣血（包含无敌时间检查）
    public void TryTakeDamage(float amount)
    {
        // 如果距离上次受伤还不到无敌时间，忽略这次伤害
        if (Time.time < lastDamageTime + invincibilityTime)
        {
            Debug.Log("受伤太频繁，无效");
            return;
        }

        // 更新受伤时间
        lastDamageTime = Time.time;
        
        // 调用真正的扣血方法
        TakeDamage(amount);
    }

    // =========================================================
    // 区域 B: 核心血量逻辑 (手动/自动通用)
    // =========================================================

    // 公开方法：可以直接被其他脚本调用 (比如掉进陷阱 TakeDamage(10))
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // 限制血量在 0 到 Max 之间
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"玩家受伤: {amount}, 剩余: {currentHealth}");

        // 通知 UI 变化
        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 辅助方法：发送通知给 HealthManager
    private void UpdateUI()
    {
        // 如果有脚本监听这个事件，就广播出去
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        
        // 死亡逻辑范例：
        // 1. 禁用玩家控制
        // 2. 播放死亡动画
        // 3. 此时可以将玩家隐藏
        // gameObject.SetActive(false); 
    }
}