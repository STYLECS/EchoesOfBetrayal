using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float backwardMultiplier = 0.6f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Double Jump")]
    public int maxJumps = 2;
    private int jumpCount;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing;
    private float dashDirection;

    private Rigidbody2D rb;
    private Animator anim;

    private float uiInput;
    private float keyboardInput;
    private float moveInput;

    private bool wasGrounded;
    private bool uiJumpPressed;
    private bool jumpQueued;

    private bool isFacingRight = true;
    private bool isSliding = false;

    private int defaultLayer;
    private int dashLayer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        defaultLayer = LayerMask.NameToLayer("Player");
        dashLayer = LayerMask.NameToLayer("PlayerDash");

        jumpCount = maxJumps;
    }


    void Update()
    {
        HandleInput();
        HandleJump();
        HandleDash();
        HandleFlip();
        UpdateAnimator();
    }


    void FixedUpdate()
    {
        if (isDashing)
            HandleDashMovement();
        else
            HandleNormalMovement();
    }


    // ================================
    //            INPUT
    // ================================
    void HandleInput()
    {
        keyboardInput =
            Input.GetKey(KeyCode.A) ? -1 :
            Input.GetKey(KeyCode.D) ? 1 : 0;

        moveInput = uiInput != 0 ? uiInput : keyboardInput;
    }


    // ================================
    //            JUMP
    // ================================
    void HandleJump()
    {
        bool grounded = IsGrounded();

        if (grounded && !wasGrounded)
            jumpCount = maxJumps;

        if (uiJumpPressed)
        {
            jumpQueued = true;
            uiJumpPressed = false;
        }

        if ((Input.GetButtonDown("Jump") ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            jumpQueued)
            && jumpCount > 0 && !isDashing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpQueued = false;
            jumpCount--;
        }

        wasGrounded = grounded;
    }


    // ================================
    //            DASH
    // ================================
    void HandleDash()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if ((Input.GetKeyDown(KeyCode.LeftShift)) &&
            dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
        }
#endif

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;
    }


    void StartDash()
    {
        isDashing = true;
        isSliding = true;              // 🔥 aktifkan slide
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = moveInput != 0 ? moveInput :
                        (isFacingRight ? 1 : -1);

        gameObject.layer = dashLayer;
    }


    void HandleDashMovement()
    {
        dashTimer -= Time.fixedDeltaTime;

        float t = 1f - (dashTimer / dashDuration);
        float currentSpeed = Mathf.Lerp(moveSpeed, dashSpeed, t);

        rb.velocity = new Vector2(dashDirection * currentSpeed, rb.velocity.y);

        if (dashTimer <= 0f)
            StopDash();
    }


    void StopDash()
    {
        isDashing = false;
        isSliding = false;           // 🔥 matikan slide
        gameObject.layer = defaultLayer;
    }


    // ================================
    //           MOVEMENT
    // ================================
    void HandleNormalMovement()
    {
        float facingDirection = Mathf.Sign(transform.localScale.x);
        bool movingBackward = (moveInput != 0 && Mathf.Sign(moveInput) != facingDirection);

        float currentSpeed = moveSpeed * (movingBackward ? backwardMultiplier : 1f);

        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
    }


    // ================================
    //           FLIP
    // ================================
    void HandleFlip()
    {
        if (moveInput > 0 && !isFacingRight)
            Flip();
        else if (moveInput < 0 && isFacingRight)
            Flip();
    }


    public void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }


    // ================================
    //           ANIMATOR
    // ================================
    void UpdateAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("isJumping", !IsGrounded());
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isRunning", Mathf.Abs(moveInput) > 0.1f && IsGrounded());
    }


    // ================================
    //           UTILITIES
    // ================================
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


    // PUBLIC UI BUTTON EVENTS
    public void KananTekan() { uiInput = 1; if (!isFacingRight) Flip(); }
    public void KananLepas() { uiInput = 0; }
    public void KiriTekan() { uiInput = -1; if (isFacingRight) Flip(); }
    public void KiriLepas() { uiInput = 0; }
    public void LompatTekan() { uiJumpPressed = true; }
    public void DashTekan()
    {
        if (dashCooldownTimer <= 0f && !isDashing)
            StartDash();
    }

    public float Horizontal => moveInput;
    public bool IsFacingRight => isFacingRight;
}
