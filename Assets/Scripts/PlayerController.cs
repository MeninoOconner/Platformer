using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float moveSpeed = 7f;
    public float jumpForce = 12f;

    [Header("Chão")]
    public LayerMask groundLayer;
    public float groundCheckHeight = 0.15f;
    public float jumpLockTime = 0.15f;

    [Header("Sensação do pulo")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float moveInput;
    private bool isGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float lastJumpTime = -10f;
    private bool facingRight = true;

    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        PhysicsMaterial2D noFriction = new PhysicsMaterial2D("SemAtrito");
        noFriction.friction = 0f;
        noFriction.bounciness = 0f;
        col.sharedMaterial = noFriction;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = (Time.time - lastJumpTime > jumpLockTime) && CheckGround();

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            lastJumpTime = Time.time;
            isGrounded = false;
        }

        if (moveInput > 0f && !facingRight)
            Flip();
        else if (moveInput < 0f && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    bool CheckGround()
    {
        Bounds b = col.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - groundCheckHeight * 0.5f);
        Vector2 size = new Vector2(b.size.x * 0.9f, groundCheckHeight);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, groundLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == col) continue;
            if (hit.isTrigger) continue;
            if (hit.transform.IsChildOf(transform)) continue;

            return true;
        }

        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        Bounds b = c.bounds;
        Vector3 center = new Vector3(b.center.x, b.min.y - groundCheckHeight * 0.5f, 0f);
        Vector3 size = new Vector3(b.size.x * 0.9f, groundCheckHeight, 0f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }
}