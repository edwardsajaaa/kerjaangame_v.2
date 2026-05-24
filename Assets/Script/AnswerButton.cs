using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerButton : MonoBehaviour
{
    [SerializeField] private int answerIndex;
    [SerializeField] private QuizManager quizManager;
    
    private Image buttonImage;
    private Button button;
    private Color originalColor;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;

        // Auto-find QuizManager jika belum di-assign
        if (quizManager == null)
        {
            quizManager = FindObjectOfType<QuizManager>();
        }
    }

    // Fungsi untuk reset warna tombol
    public void ResetButtonColor()
    {
        buttonImage.color = originalColor;
    }

    // Fungsi untuk ubah warna
    public void SetButtonColor(Color newColor)
    {
        buttonImage.color = newColor;
    }
}
