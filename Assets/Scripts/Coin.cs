using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(value);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoin();
            }

            Destroy(gameObject);
        }
    }
}