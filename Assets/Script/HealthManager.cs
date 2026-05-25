using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Setup health bar
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            currentHealth = maxHealth;
            UpdateHealthDisplay();
        }
        else
        {
            currentHealth = maxHealth;
            Debug.LogWarning("Health Slider tidak di-assign di HealthManager!");
        }

        // Hide gameover panel saat start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Game Over Panel tidak di-assign di HealthManager!");
        }

        Debug.Log($"[HEALTH] Initialized - Health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Kurangi health saat kena water / salah jawab quiz timer
    /// </summary>
    public void DecreaseHealth(int amount = 1)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth); // Jangan bisa minus

        Debug.Log($"💔 Health berkurang {amount}! Current: {currentHealth}/{maxHealth}");
        UpdateHealthDisplay();

        // Check jika health habis
        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Tambah health (healing)
    /// </summary>
    public void IncreaseHealth(int amount = 1)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // Max health

        Debug.Log($"💚 Health bertambah {amount}! Current: {currentHealth}/{maxHealth}");
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("💀 GAME OVER! Health habis!");

        // Stop & simpan gameplay timer
        if (GameplayTimer.instance != null)
        {
            GameplayTimer.instance.StopAndSaveTime();
        }

        // Tutup quiz panel jika sedang terbuka
        QuizManager[] quizManagers = FindObjectsOfType<QuizManager>(true);
        foreach (QuizManager qm in quizManagers)
        {
            qm.gameObject.SetActive(false);
        }

        // Show gameover panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Freeze game
        Time.timeScale = 0f;
        Debug.Log("⏸️ Game paused (Time.timeScale = 0)");
    }

    // Getter functions
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    /// <summary>
    /// Restart game dari awal scene (untuk button Retry di Game Over Panel)
    /// </summary>
    public void RestartGame()
    {
        // Unfreeze game dulu sebelum reload
        Time.timeScale = 1f;

        // Reload scene dari awal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("🔄 Restarting scene dari awal!");
    }
}
