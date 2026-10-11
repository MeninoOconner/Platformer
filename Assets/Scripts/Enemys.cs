using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrulha")]
    public float speed = 2f;
    public float patrolDistance = 3f;
    public bool spriteFacesRight = true;

    [Header("Combate")]
    public bool canBeStomped = true;
    public float stompTolerance = 0.25f;
    public float bounceForce = 8f;

    private Vector3 startPosition;
    private int direction = 1;
    private float baseScaleX;
    private Collider2D col;

    void Start()
    {
        startPosition = transform.position;
        baseScaleX = Mathf.Abs(transform.localScale.x);
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.position.x >= startPosition.x + patrolDistance)
        {
            direction = -1;
        }
        else if (transform.position.x <= startPosition.x - patrolDistance)
        {
            direction = 1;
        }

        float facing = spriteFacesRight ? direction : -direction;
        Vector3 scale = transform.localScale;
        scale.x = baseScaleX * facing;
        transform.localScale = scale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        bool stomped = false;

        if (canBeStomped)
        {
            Bounds playerBounds = collision.collider.bounds;
            Bounds enemyBounds = col.bounds;
            stomped = playerBounds.min.y >= enemyBounds.max.y - stompTolerance;
        }

        if (stomped)
        {
            Rigidbody2D playerBody = collision.collider.attachedRigidbody;
            if (playerBody != null)
            {
                playerBody.linearVelocity = new Vector2(playerBody.linearVelocity.x, bounceForce);
            }

            Destroy(gameObject);
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseLife();
            }
        }
    }
}