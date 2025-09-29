using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f;   // durasi game
    private float currentTime;
    public TextMeshProUGUI timerText;

    [Header("Score")]
    public TextMeshProUGUI scoreText;
    private int score = 0;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;

    void Start()
    {
        currentTime = startTime;
        gameOverPanel.SetActive(false);
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return;

        // Kurangi timer
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            EndGame();
        }

        // Update tampilan timer
        timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Sampah: " + score;
    }

    void EndGame()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);

        // Ambil tulisan dari CurrentScoreText dan masukkan ke panel GameOver
        finalScoreText.text = "Total sampah yang anda kumpulkan adalah " 
                           + scoreText.text;
    } 


    // Tombol UI
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("menu"); // nama home
    }
}
