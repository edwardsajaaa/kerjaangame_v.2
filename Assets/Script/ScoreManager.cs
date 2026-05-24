using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton
    
    [SerializeField] private TextMeshProUGUI scoreDisplay; // UI untuk tampilkan poin
    [SerializeField] private int totalQuestions = 9;
    [SerializeField] private int maxScore = 100;
    
    private int currentScore = 0;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        UpdateScoreDisplay();
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

    // Get poin per pertanyaan
    public int GetPointsPerQuestion()
    {
        return maxScore / totalQuestions; // 100 / 9 = 11 poin per soal
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

    // Set max score dan total questions
    public void SetScoreConfig(int newMaxScore, int newTotalQuestions)
    {
        maxScore = newMaxScore;
        totalQuestions = newTotalQuestions;
        UpdateScoreDisplay();
    }
}
