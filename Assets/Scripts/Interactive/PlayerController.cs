using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 7f;        // ���������ٶ�
    [SerializeField] private float jumpForce = 7f;        // ��Ծ����
    [SerializeField] private float fallMultiplier = 2.5f; // ׹����ٶȱ���
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;       // �����
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip jumpAudioClip;  // ��Ծ��Ч
    [SerializeField] private AudioClip windAudioClip;  // ������Ч
    [SerializeField] private AudioClip fallAudioClip;  // ������Ч
    [SerializeField] private AudioClip runAudioClip;   // �ܲ���Ч
    private AudioSource audioSource;                   // ���ڲ�����Ч
    private AudioSource runAudioSource;                   // ����ѭ�������ܲ���Ч

    [Header("Events")]
    public UnityEvent onSuccessfulJump;  // �ɹ���Ծ�¼�
    public UnityEvent onInteract;        // �����¼�
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool canMove = true;
    private float lastJumpTime;
    private const float SUCCESSFUL_JUMP_HEIGHT = 2f; // �ж��ɹ���Ծ�ĸ߶�
    private float initialY;
    private bool hasTriggeredJumpSuccess;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialY = transform.position.y;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // �ܲ���Чר��AudioSource
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
        CheckSuccessfulJump();
    }

    private void CheckGrounded()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null) return;

        // 使用 BoxCast 进行地面检测
        Vector2 boxCenter = new Vector2(playerCollider.bounds.center.x, playerCollider.bounds.min.y);

        Vector2 boxSize = new Vector2(playerCollider.bounds.size.x * 0.9f, 0.1f); float castDistance = 0.1f;

        RaycastHit2D hit = Physics2D.BoxCast(boxCenter, boxSize, 0f, Vector2.down, castDistance, groundLayer);

        isGrounded = hit.collider != null;

    }

    private void HandleMovement()
    {
        if (!canMove) return;

        float moveInput = 0f;
        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady)
        {
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveLeft))
                moveInput -= 1f;
            if (KeymapManager.Singleton.IsKeyHeld(KeymapManager.Function.MoveRight))
                moveInput += 1f;
        }

        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y);

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

        if (KeymapManager.Singleton != null && KeymapManager.Singleton.IsReady &&
            KeymapManager.Singleton.IsKeyPressed(KeymapManager.Function.Interact))
        {
            onInteract?.Invoke();
        }

        // ׹�����
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
