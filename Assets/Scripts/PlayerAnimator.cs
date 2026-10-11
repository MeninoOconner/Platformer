using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite jumpSprite;
    public Sprite[] walkSprites;

    [Header("Animação")]
    public float walkFrameRate = 10f;
    public float walkThreshold = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerController controller;
    private float walkTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        bool inAir = controller != null && !controller.IsGrounded;
        bool walking = Mathf.Abs(rb.linearVelocity.x) > walkThreshold;
        bool hasWalk = walkSprites != null && walkSprites.Length > 0;

        Sprite standing = idleSprite != null ? idleSprite : (hasWalk ? walkSprites[0] : null);

        if (inAir)
        {
            walkTimer = 0f;

            Sprite airSprite = jumpSprite != null ? jumpSprite : standing;
            if (airSprite != null)
            {
                spriteRenderer.sprite = airSprite;
            }
        }
        else if (walking && hasWalk)
        {
            walkTimer += Time.deltaTime * walkFrameRate;
            int frame = (int)walkTimer % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[frame];
        }
        else
        {
            walkTimer = 0f;

            if (standing != null)
            {
                spriteRenderer.sprite = standing;
            }
        }
    }
}