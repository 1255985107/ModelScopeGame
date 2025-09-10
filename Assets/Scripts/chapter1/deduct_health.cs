using System.Collections;
using UnityEngine;

public class LavaPool : MonoBehaviour
{
    public int damagePerTick = 1;       // ÿ�ο۷�
    public float tickInterval = 1f;     // �۷ּ�����룩

    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ��ʼ�����۷�
            damageCoroutine = StartCoroutine(DamageOverTime(other.gameObject));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ֹͣ�����۷�
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
        }
    }

    private IEnumerator DamageOverTime(GameObject player)
    {
        // ��������� PlayerHealth �ű�
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
