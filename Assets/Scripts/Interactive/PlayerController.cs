using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;        // ���������ٶ�
    [SerializeField] private float jumpForce = 5f;        // ��Ծ����
    [SerializeField] private float fallMultiplier = 2.5f; // ׹����ٶȱ���
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;       // �����
    [SerializeField] private float groundCheckRadius = 0.05f;
    [SerializeField] private float deathYThreshold = -10f; // �����߶���ֵ

    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckCenter;
    [SerializeField] private Transform groundCheckRight;

    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    
    [Header("Audio")]
    [SerializeField] private AudioClip jumpAudioClip;  // ��Ծ��Ч
    [SerializeField] private AudioClip windAudioClip;  // ������Ч
    [SerializeField] private AudioClip fallAudioClip;  // ������Ч
    [SerializeField] private AudioClip runAudioClip;   // �ܲ���Ч
    private AudioSource audioSource;                   // ���ڲ�����Ч
    private AudioSource runAudioSource;                   // ����ѭ�������ܲ���Ч

    [Header("Events")]
    public UnityEvent onDeath;           // �����¼�
    public UnityEvent onSuccessfulJump;  // �ɹ���Ծ�¼�
    public UnityEvent onInteract;        // �����¼�
    
    private Rigidbody2D rb;
    public bool isGrounded;

    public bool hit_left_wall;
    public bool hit_right_wall;
    private bool canJump = true;
    private bool canMove = true;
    private float lastJumpTime;
    private const float SUCCESSFUL_JUMP_HEIGHT = 2f; // �ж��ɹ���Ծ�ĸ߶�
    private float initialY;
    private bool hasTriggeredJumpSuccess;

    private float moveDelayTimer = 0f;
    private bool moveKeyPressed = false;
    public float moveInput;
    public float jumpInput;

    public bool isJumpingState = false;      // 用于动画系统
    private bool jumpTriggered = false;      // 用于传递跳跃信号给FixedUpdate
    private float coyoteTime = 0.1f;
    private float lastGroundedTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;          
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;           
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        initialY = transform.position.y;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        runAudioSource = gameObject.AddComponent<AudioSource>();
        runAudioSource.clip = runAudioClip;
        runAudioSource.loop = true;
        runAudioSource.playOnAwake = false;
    }

    void Update()
    {
        CheckGrounded();
        UpdateJumpState();   // 新增：更新跳跃动画状态
        HandleMovement();
        HandleRunSound();
        CheckDeath();
        CheckSuccessfulJump();
    }

    void FixedUpdate()
    {
        // ✅ 正确：在FixedUpdate中执行物理操作
        if (jumpTriggered)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTriggered = false;  // 重置信号
        }
    }

private bool wasGrounded = false;
private int groundedFrameCount = 0;
private const int GROUNDED_FRAME_THRESHOLD = 2;  // 需要连续2帧才确认离地

private void CheckGrounded()
{
    Collider2D playerCollider = GetComponent<Collider2D>();
    float playerBottom = playerCollider.bounds.min.y;
    
    // 使用 Raycast 替代 OverlapCircle，更稳定
    bool hitGround = false;
    
    // 从三个点向下发射射线
    RaycastHit2D hitLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D hitCenter = Physics2D.Raycast(groundCheckCenter.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D hitRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D wallhitLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, groundCheckRadius, groundLayer);
    RaycastHit2D wallhitRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, groundCheckRadius, groundLayer);

    hitGround = hitLeft.collider != null || hitCenter.collider != null || hitRight.collider != null;
    hit_left_wall = wallhitLeft.collider != null;
    hit_right_wall = wallhitRight.collider != null;
    // 添加缓冲逻辑，防止单帧抖动
    if (hitGround)
    {
        groundedFrameCount = GROUNDED_FRAME_THRESHOLD;
        isGrounded = true;
        wasGrounded = true;
    }
    else
    {
        if (groundedFrameCount > 0)
        {
            groundedFrameCount--;
            isGrounded = true;  // 短时间内仍然视为在地面
        }
        else
        {
            isGrounded = false;
            wasGrounded = false;
        }
    }
    
    // 记录最后在地面的时间
    if (isGrounded)
    {
        lastGroundedTime = Time.time;
    }
}

    // 新增：更新跳跃动画状态（用于AnimatorTrigger读取）
    private void UpdateJumpState()
    {
        // 在地面上且垂直速度接近0 → 不在跳跃
        if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.5f)
        {
            isJumpingState = false;
        }
        
        // 垂直速度向上且不在地面 → 正在跳跃
        if (!isGrounded && rb.velocity.y > 0.5f)
        {
            isJumpingState = true;
        }
        
        // 刚离开地面就快速下落 → 滑落而非跳跃
        if (!isGrounded && Time.time - lastGroundedTime < coyoteTime && rb.velocity.y < -1f)
        {
            isJumpingState = false;
        }
    }

    private void HandleMovement()
    {
        if (!canMove) return;

        moveInput = 0f;

        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            if (moveDelayTimer > 0)
            {
                moveDelayTimer -= Time.deltaTime;
            }
            else
            {
                if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
                    moveInput -= 1f;
                if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
                    moveInput += 1f;
            }

            if (KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveLeft) ||
                KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveRight))
            {
                moveDelayTimer = 0.05f;
            }
        }
        if (!hit_left_wall && !hit_right_wall)
        {
            rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
        }
        else if (hit_left_wall)
        {
            if (moveInput > 0)
                rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        else
        {
            if (moveInput < 0)
                rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);
            else
                rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // ✅ 检测跳跃输入，设置信号
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady &&
            KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp) && isGrounded && canJump)
        {
            jumpTriggered = true;     // 设置跳跃信号给FixedUpdate
            isJumpingState = true;    // 设置跳跃状态给AnimatorTrigger
            lastJumpTime = Time.time;
            hasTriggeredJumpSuccess = false;

            if (jumpAudioClip != null)
            {
                audioSource.PlayOneShot(jumpAudioClip);
            }
        }
    }

    private void HandleRunSound()
    {
        moveInput = 0f;
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
            //ֹͣ׹��
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
