using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;        // 慢速行走速度
    [SerializeField] private float jumpForce = 5f;        // 跳跃力度
    [SerializeField] private float fallMultiplier = 2.5f; // 坠落加速度倍数
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;       // 地面层
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float deathYThreshold = -10f; // 死亡高度阈值
    
    [Header("Audio")]
    [SerializeField] private AudioClip jumpAudioClip;  // 跳跃音效
    [SerializeField] private AudioClip windAudioClip;  // 风声音效
    [SerializeField] private AudioClip fallAudioClip;  // 跌落音效
    [SerializeField] private AudioClip runAudioClip;   // 跑步音效
    private AudioSource audioSource;                   // 用于播放音效
    private AudioSource runAudioSource;                   // 用于循环播放跑步音效

    [Header("Events")]
    public UnityEvent onDeath;           // 死亡事件
    public UnityEvent onSuccessfulJump;  // 成功跳跃事件
    
    private Rigidbody2D rb;
    public bool isGrounded;
    private bool canJump = true;
    private bool canMove = true;
    private float lastJumpTime;
    private const float SUCCESSFUL_JUMP_HEIGHT = 2f; // 判定成功跳跃的高度
    private float initialY;
    private bool hasTriggeredJumpSuccess;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialY = transform.position.y;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 跑步音效专用AudioSource
        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.clip = runAudioClip;
        runAudioSource.loop = true;
        runAudioSource.playOnAwake = false;
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleRunSound();
        CheckDeath();
        UpdateWindSound();
        CheckSuccessfulJump();
    }

    private void CheckGrounded()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        float playerBottom = playerCollider.bounds.min.y;
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, playerBottom - 0.1f), groundCheckRadius, groundLayer);
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        // 优化后的输入检测
        float moveInput = 0f;
        //Debug.Log($"Checking input: {KeymapManager.Singleton.IsReady}");
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
                moveInput -= 1f;
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
                moveInput += 1f;
        }

        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);

        // 跳跃控制
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady &&
            KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp) && isGrounded && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            lastJumpTime = Time.time;
            hasTriggeredJumpSuccess = false;

            if (jumpAudioClip != null)
            {
                audioSource.PlayOneShot(jumpAudioClip);
            }
        }

        // 坠落加速
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleRunSound()
    {
        float moveInput = 0f;
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
                moveInput -= 1f;
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
                moveInput += 1f;
        }
        bool shouldPlayRun = isGrounded && canMove && Mathf.Abs(moveInput) > 0.1f;

        if (shouldPlayRun && runAudioClip != null)
        {
            if (!runAudioSource.isPlaying)
                runAudioSource.Play();
        }
        else
        {
            if (runAudioSource.isPlaying)
                runAudioSource.Stop();
        }
    }

    private void UpdateWindSound()
    {
        // 风声建议用独立AudioSource循环播放，这里仅做示例
        // 可根据需要扩展为独立风声AudioSource
    }

    private void CheckSuccessfulJump()
    {
        if (!hasTriggeredJumpSuccess && Time.time - lastJumpTime > 0.5f)
        {
            float heightGained = transform.position.y - initialY;
            if (heightGained >= SUCCESSFUL_JUMP_HEIGHT)
            {
                hasTriggeredJumpSuccess = true;
                onSuccessfulJump?.Invoke();
            }
        }
    }

    private void CheckDeath()
    {
        if (transform.position.y < deathYThreshold)
        {
            if (fallAudioClip != null)
            {
                audioSource.PlayOneShot(fallAudioClip);
            }
            //停止坠落
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            onDeath?.Invoke();
        }
    }

    public void DisableJump()
    {
        canJump = false;
    }

    public void EnableJump()
    {
        canJump = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        if (runAudioSource.isPlaying)
            runAudioSource.Stop();
    }

    public void EnableMovement()
    {
        canMove = true;
    }
}
