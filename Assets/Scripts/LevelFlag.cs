using UnityEngine;

public class LevelFlag : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Algo tocou na bandeira: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                Debug.Log("Chamando WinLevel");
                GameManager.Instance.WinLevel();
            }
            else
            {
                Debug.Log("GameManager.Instance está vazio");
            }
        }
    }
}