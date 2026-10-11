using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public TextMeshProUGUI winScoreText;

    [Header("Jogador")]
    public Transform player;
    public Transform spawnPoint;
    public int startingLives = 3;
    public float invincibleTime = 1f;

    private int score = 0;
    private int lives;
    private float invincibleUntil = 0f;
    private bool gameOver = false;
    private bool levelWon = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        lives = startingLives;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        if (gameOver || levelWon) return;

        score += amount;
        UpdateUI();
    }

    public void LoseLife()
    {
        if (gameOver || levelWon) return;
        if (Time.time < invincibleUntil) return;

        lives--;
        invincibleUntil = Time.time + invincibleTime;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayHurt();
            }

            Respawn();
        }
    }

    public void WinLevel()
    {
        if (gameOver || levelWon) return;

        levelWon = true;

        if (winScoreText != null)
        {
            winScoreText.text = "Moedas: " + score;
        }

        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayWin();
        }

        Time.timeScale = 0f;
    }

    void Respawn()
    {
        if (player == null || spawnPoint == null) return;

        player.position = spawnPoint.position;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void GameOver()
    {
        gameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.PlayGameOver();
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Moedas: " + score;
        }

        if (livesText != null)
        {
            livesText.text = "Vidas: " + lives;
        }
    }
}