using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;        // 锟斤拷锟斤拷锟斤拷锟斤拷锟劫讹拷
    [SerializeField] private float jumpForce = 5f;        // 锟斤拷跃锟斤拷锟斤拷
    [SerializeField] private float fallMultiplier = 2.5f; // 坠锟斤拷锟斤拷俣缺锟斤拷锟�
    
    [Header("Double Jump Settings")]
    [SerializeField] private bool enableDoubleJump = true;      // 鏄惁鍚敤浜屾璺�
    [SerializeField] private float doubleJumpForce = 4.5f;      // 浜屾璺冲姏搴�
    [SerializeField] private int maxJumpCount = 2;              // 鏈€澶ц烦璺冩鏁�
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;       // 锟斤拷锟斤拷锟�
    [SerializeField] private float groundCheckRadius = 0.05f;
    [SerializeField] private float deathYThreshold = -100f; // 锟斤拷锟斤拷锟竭讹拷锟斤拷值

    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckCenter;
    [SerializeField] private Transform groundCheckRight;

    [SerializeField] private Transform wallCheckLeft;
    [SerializeField] private Transform wallCheckRight;
    
    [Header("Audio")]
    [SerializeField] private AudioClip jumpAudioClip;  // 锟斤拷跃锟斤拷效
    [SerializeField] private AudioClip windAudioClip;  // 锟斤拷锟斤拷锟斤拷效
    [SerializeField] private AudioClip fallAudioClip;  // 锟斤拷锟斤拷锟斤拷效
    [SerializeField] private AudioClip runAudioClip;   // 锟杰诧拷锟斤拷效
    private AudioSource audioSource;                   // 锟斤拷锟节诧拷锟斤拷锟斤拷效
    private AudioSource runAudioSource;                   // 锟斤拷锟斤拷循锟斤拷锟斤拷锟斤拷锟杰诧拷锟斤拷效

    [Header("Events")]
    public UnityEvent onDeath;           // 锟斤拷锟斤拷锟铰硷拷
    public UnityEvent onSuccessfulJump;  // 锟缴癸拷锟斤拷跃锟铰硷拷
    public UnityEvent onInteract;        // 锟斤拷锟斤拷锟铰硷拷
    
    private Rigidbody2D rb;
    public bool isGrounded;

    public bool hit_left_wall;
    public bool hit_right_wall;
    private bool canJump = true;
    private bool canMove = true;
    private float lastJumpTime;
    private const float SUCCESSFUL_JUMP_HEIGHT = 2f; // 锟叫讹拷锟缴癸拷锟斤拷跃锟侥高讹拷
    private float initialY;
    private bool hasTriggeredJumpSuccess;

    private float moveDelayTimer = 0f;
    private bool moveKeyPressed = false;
    public float moveInput;
    public float jumpInput;

    public bool isJumpingState = false;      // 鐢ㄤ簬鍔ㄧ敾绯荤粺
    private bool jumpTriggered = false;      // 鐢ㄤ簬浼犻€掕烦璺冧俊鍙风粰FixedUpdate
    private float jumpForceToApply;          
    private int currentJumpCount = 0;        
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
        if (ChatManager.IsUIFocused)
        {
            return; 
        }
        CheckGrounded();
        UpdateJumpState();   // 鏂板锛氭洿鏂拌烦璺冨姩鐢荤姸鎬�
        HandleMovement();
        HandleRunSound();
        CheckDeath();
        CheckSuccessfulJump();
    }

    void FixedUpdate()
    {
        // 鉁� 姝ｇ‘锛氬湪FixedUpdate涓墽琛岀墿鐞嗘搷浣�
        if (jumpTriggered)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForceToApply);
            jumpTriggered = false;  // 閲嶇疆淇″彿
        }
    }

private bool wasGrounded = false;
private int groundedFrameCount = 0;
private const int GROUNDED_FRAME_THRESHOLD = 2;  // 闇€瑕佽繛缁�2甯ф墠纭绂诲湴

private void CheckGrounded()
{
    Collider2D playerCollider = GetComponent<Collider2D>();
    float playerBottom = playerCollider.bounds.min.y;
    
    // 浣跨敤 Raycast 鏇夸唬 OverlapCircle锛屾洿绋冲畾
    bool hitGround = false;
    
    // 浠庝笁涓偣鍚戜笅鍙戝皠灏勭嚎
    RaycastHit2D hitLeft = Physics2D.Raycast(groundCheckLeft.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D hitCenter = Physics2D.Raycast(groundCheckCenter.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D hitRight = Physics2D.Raycast(groundCheckRight.position, Vector2.down, groundCheckRadius, groundLayer);
    RaycastHit2D wallhitLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, groundCheckRadius, groundLayer);
    RaycastHit2D wallhitRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, groundCheckRadius, groundLayer);

    hitGround = hitLeft.collider != null || hitCenter.collider != null || hitRight.collider != null;
    hit_left_wall = wallhitLeft.collider != null;
    hit_right_wall = wallhitRight.collider != null;
    // 娣诲姞缂撳啿閫昏緫锛岄槻姝㈠崟甯ф姈鍔�
    if (hitGround)
    {
        groundedFrameCount = GROUNDED_FRAME_THRESHOLD;
        isGrounded = true;
        wasGrounded = true;
        currentJumpCount = 0;  // 钀藉湴鏃堕噸缃烦璺冩鏁�
    }
    else
    {
        if (groundedFrameCount > 0)
        {
            groundedFrameCount--;
            isGrounded = true;  // 鐭椂闂村唴浠嶇劧瑙嗕负鍦ㄥ湴闈�
        }
        else
        {
            isGrounded = false;
            wasGrounded = false;
        }
    }
    
    // 璁板綍鏈€鍚庡湪鍦伴潰鐨勬椂闂�
    if (isGrounded)
    {
        lastGroundedTime = Time.time;
    }
}

    private void UpdateJumpState()
    {
        if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.5f)
        {
            isJumpingState = false;
        }
        
        if (!isGrounded && rb.velocity.y > 0.5f)
        {
            isJumpingState = true;
        }
        
        // 鍒氱寮€鍦伴潰灏卞揩閫熶笅钀� 鈫� 婊戣惤鑰岄潪璺宠穬
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

        //  妫€娴嬭烦璺冭緭鍏ワ紝鏀寔浜屾璺�
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady &&
            KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.MoveUp) && canJump)
        {
            bool canPerformJump = false;
            bool isDoubleJump = false;
            
            // 涓€娈佃烦锛氬湪鍦伴潰涓�
            if (isGrounded && currentJumpCount == 0)
            {
                canPerformJump = true;
                jumpForceToApply = jumpForce;
                currentJumpCount = 1;
            }
            // 浜屾璺筹細鍦ㄧ┖涓笖鍚敤浜屾璺充笖杩樻湁璺宠穬娆℃暟
            else if (enableDoubleJump && !isGrounded && currentJumpCount < maxJumpCount)
            {
                canPerformJump = true;
                isDoubleJump = true;
                jumpForceToApply = doubleJumpForce;
                currentJumpCount++;
            }
            
            if (canPerformJump)
            {
                jumpTriggered = true;     // 璁剧疆璺宠穬淇″彿缁橣ixedUpdate
                isJumpingState = true;    // 璁剧疆璺宠穬鐘舵€佺粰AnimatorTrigger
                lastJumpTime = Time.time;
                hasTriggeredJumpSuccess = false;

                // 鎾斁闊虫晥
                AudioClip clipToPlay = jumpAudioClip;
                if (clipToPlay != null)
                {
                    audioSource.PlayOneShot(clipToPlay);
                }
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
            //停止坠锟斤拷
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

    public void SetDoubleJumpEnabled(bool enabled)
    {
        enableDoubleJump = enabled;
    }

    public bool IsDoubleJumpEnabled()
    {
        return enableDoubleJump;
    }

    public void SetMaxJumpCount(int count)
    {
        maxJumpCount = Mathf.Max(1, count);
    }

    public int GetRemainingJumps()
    {
        return maxJumpCount - currentJumpCount;
    }

    public void ResetJumpCount()
    {
        currentJumpCount = 0;
    }
}
