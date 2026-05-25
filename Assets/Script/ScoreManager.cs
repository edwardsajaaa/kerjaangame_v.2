using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton
    
    [SerializeField] private TextMeshProUGUI scoreDisplay; // UI untuk tampilkan poin
    [SerializeField] private int totalQuestions = 9;  // Ubah ke 18 untuk Level 2
    [SerializeField] private int maxScore = 9;        // Ubah ke 18 untuk Level 2
    
    private int currentScore = 0;

    void Awake()
    {
        // Singleton per-scene (TIDAK DontDestroyOnLoad)
        // Setiap level punya ScoreManager sendiri, fresh dari 0
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Disable debug runtime UI agar tidak blocking input
        DebugManager.instance.enableRuntimeUI = false;
        Debug.Log("[SCORE] Debug UI disabled - button input should work now!");
    }

    void Start()
    {
        currentScore = 0; // Selalu mulai dari 0 saat scene baru
        UpdateScoreDisplay();
        Debug.Log($"[SCORE] Level dimulai - Max: {maxScore}, Total Soal: {totalQuestions}");
    }

    // Tambah poin ketika jawab benar
    public void AddPoints(int points)
    {
        currentScore += points;
        
        // Pastikan tidak lebih dari max
        if (currentScore > maxScore)
        {
            currentScore = maxScore;
        }
        
        UpdateScoreDisplay();
        Debug.Log($"[SCORE] +{points} | Total: {currentScore}/{maxScore}");
    }

    // Update tampilan poin di UI
    void UpdateScoreDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.text = $"{currentScore}/{maxScore}";
        }
    }

    // Get poin per pertanyaan (1 soal = 1 poin)
    public int GetPointsPerQuestion()
    {
        return 1;
    }

    // Get total poin saat ini
    public int GetCurrentScore()
    {
        return currentScore;
    }

    // Reset score (untuk restart game)
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
        Debug.Log("[SCORE] Reset to 0");
    }

    // Set max score dan total questions (jika perlu diubah runtime)
    public void SetScoreConfig(int newMaxScore, int newTotalQuestions)
    {
        maxScore = newMaxScore;
        totalQuestions = newTotalQuestions;
        UpdateScoreDisplay();
    }
}
