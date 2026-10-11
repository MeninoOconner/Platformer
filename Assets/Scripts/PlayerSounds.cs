using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSounds : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody2D rb;
    private bool wasGrounded;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool grounded = controller.IsGrounded;

        if (wasGrounded && !grounded && rb.linearVelocity.y > 1f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJump();
            }
        }

        wasGrounded = grounded;
    }
}