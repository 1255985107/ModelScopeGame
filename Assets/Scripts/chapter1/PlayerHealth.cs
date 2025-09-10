using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // 最大生命值
    public int currentHealth;

    private void Start()
    {
        currentHealth = 12;  // 初始化血量
    }

    // 扣血/扣分方法
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);  // 防止血量小于0
        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // 可以加死亡动画、重生逻辑等
    }

    private void Update()
    {
        //Debug.Log("Current Health: " + currentHealth);
    }
}
