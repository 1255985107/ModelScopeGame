using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;  // �������ֵ
    public int currentHealth;

    private void Start()
    {
        currentHealth = 12;  // ��ʼ��Ѫ��
    }

    // ��Ѫ/�۷ַ���
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);  // ��ֹѪ��С��0
        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // ���Լ����������������߼���
    }

    private void Update()
    {
        //Debug.Log("Current Health: " + currentHealth);
    }
}
