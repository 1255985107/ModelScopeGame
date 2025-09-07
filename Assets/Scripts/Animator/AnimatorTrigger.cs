using UnityEngine;
public class AnimatorTrigger : MonoBehaviour
{
    [Header("References")]
    private PlayerController playerController;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Animation Parameters")]
    private readonly string PARAM_IS_WALKING = "IsWalking";
    private readonly string PARAM_IS_JUMPING = "IsJumping";
    private readonly string PARAM_IS_IDLE = "IsIdle";
    private readonly string PARAM_IS_DEAD = "IsDead";
    // 动画参数名称（字符串常量）

    [Header("Settings")]
    // 阈值设置
    const float H_THRESHOLD = 0.05f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = FindObjectOfType<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            rb = FindObjectOfType<Rigidbody2D>();
        }

        if (playerController == null)
        {
            Debug.LogError("AnimatorTrigger: 找不到PlayerController组件!");
        }
        else
        {
            playerController.onDeath.AddListener(OnDeath);
        }

        if (animator == null)
        {
            Debug.LogError("AnimatorTrigger: 找不到Animator组件！");
        }

        if (rb == null)
        {
            Debug.LogError("AnimatorTrigger: 找不到Rigidbody2D组件！");
        }
    }

    void Update()
    {
        if (playerController == null || rb == null || animator == null)
        {
            return;
        }
        CheckMovementAnimation();
        CheckDirectionChange();
    }

    private void CheckMovementAnimation()
    {
        // 基于地面判定决定是否在空中
        bool grounded = playerController != null && playerController.isGrounded;
        float h = Mathf.Abs(rb.velocity.x);

        bool isWalking = grounded && h > H_THRESHOLD;
        bool isIdle    = grounded && h <= H_THRESHOLD;
        bool isJumping = !grounded;

        animator.SetBool(PARAM_IS_WALKING, isWalking);
        animator.SetBool(PARAM_IS_IDLE,    isIdle);
        animator.SetBool(PARAM_IS_JUMPING, isJumping);
    }

    private void CheckDirectionChange()
    {
        if (rb.velocity.x > H_THRESHOLD)
        {
            // 向右移动 - 面向右边
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (rb.velocity.x < -H_THRESHOLD)
        {
            // 向左移动 - 面向左边  
            transform.localScale = new Vector3(-1, 1, 1);
        }
        // 如果速度接近0，保持当前朝向不变
    }

    private void OnDeath()
    {
        animator.SetBool(PARAM_IS_IDLE, false);
        animator.SetBool(PARAM_IS_JUMPING, false);
        animator.SetBool(PARAM_IS_WALKING, false);
        animator.SetBool(PARAM_IS_DEAD, true);
    }
}