using UnityEngine;

public class BreakablePlatformAnimated : MonoBehaviour
{
    private Animator animator;
    private CompositeCollider2D platformCollider;
    private bool isCracked = false;
    private GameObject associatedMonster;

    void Awake()
    {
        // 获取父对象上的组件
        animator = GetComponent<Animator>();
        platformCollider = GetComponent<CompositeCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 怪物踩到，且尚未破裂
        if (collision.gameObject.CompareTag("Monster") && !isCracked)
        {
            isCracked = true;
            animator.SetTrigger("crack"); // 触发Crack动画
            associatedMonster = collision.gameObject; 
            Debug.Log("平台开裂！");
        }

        // 2. 玩家碰到平台，且平台已经破裂
        if (collision.gameObject.CompareTag("Player") && isCracked)
        {
            // 立即禁用碰撞体，让玩家掉下去
            platformCollider.enabled = false;
            if (associatedMonster != null)
            {
                associatedMonster.SetActive(false);
                Debug.Log("关联的怪物一起消失了！");
            }
            gameObject.SetActive(false);
            // 打印日志方便调试
            Debug.Log("平台破碎！玩家从任意方向接触！");
        }
    }

    public void OnShatterAnimationEnd()
    {
        // 这行代码会禁用Animator组件，让动画永久停在最后一帧
        if (animator != null)
        {
            animator.enabled = false;
        }
    }
}