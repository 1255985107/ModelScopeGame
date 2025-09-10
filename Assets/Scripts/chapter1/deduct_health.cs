using System.Collections;
using UnityEngine;

public class LavaPool : MonoBehaviour
{
    public int damagePerTick = 1;       // 每次扣分
    public float tickInterval = 1f;     // 扣分间隔（秒）

    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 开始持续扣分
            damageCoroutine = StartCoroutine(DamageOverTime(other.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 停止持续扣分
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
        }
    }

    private IEnumerator DamageOverTime(GameObject player)
    {
        // 假设玩家有 PlayerHealth 脚本
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        while (true)
        {
            if (health != null)
            {
                health.TakeDamage(damagePerTick);
            }
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
