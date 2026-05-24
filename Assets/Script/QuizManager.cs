using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public List<string> answers = new List<string>();
        public int correctAnswerIndex; // Index dari jawaban yang benar (0-3)
    }

    [SerializeField] private List<Question> questions = new List<Question>();
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> answerButtons = new List<Button>();
    [SerializeField] private GameObject quizPanel; // Panel yang akan ditutup saat selesai
    
    // Untuk warna (jika tidak pakai sprite)
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    
    // Untuk sprite custom (jika pakai gambar)
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;
    
    private int currentQuestionIndex = 0;
    private bool isAnswered = false;
    private bool quizCompleted = false; // Track jika quiz sudah diselesaikan

    void Start()
    {
        // Pastikan ada 4 button untuk jawaban
        if (answerButtons.Count != 4)
        {
            Debug.LogError("Harus ada 4 tombol jawaban!");
            return;
        }

        // Auto-assign quiz panel jika belum di-assign
        if (quizPanel == null)
        {
            quizPanel = gameObject;
        }

        // Setup button listeners
        for (int i = 0; i < answerButtons.Count; i++)
        {
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }

        // Tampilkan pertanyaan pertama
        if (questions.Count > 0)
        {
            DisplayQuestion(0);
        }
    }

    void DisplayQuestion(int questionIndex)
    {
        if (questionIndex >= questions.Count)
        {
            Debug.Log("Semua pertanyaan selesai!");
            return;
        }

        currentQuestionIndex = questionIndex;
        isAnswered = false;
        Question question = questions[questionIndex];

        // Tampilkan teks pertanyaan
        questionText.text = question.questionText;

        // Tampilkan pilihan jawaban
        for (int i = 0; i < answerButtons.Count; i++)
        {
            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = question.answers[i];
            }

            // Reset sprite atau warna tombol
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            if (defaultSprite != null)
            {
                buttonImage.sprite = defaultSprite;
            }
            else
            {
                buttonImage.color = defaultColor;
            }
        }
    }

    void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswered) return;

        isAnswered = true;
        quizCompleted = true; // Quiz sudah diselesaikan
        Question question = questions[currentQuestionIndex];
        Image selectedButtonImage = answerButtons[selectedIndex].GetComponent<Image>();

        if (selectedIndex == question.correctAnswerIndex)
        {
            // Jawaban benar
            if (correctSprite != null)
            {
                selectedButtonImage.sprite = correctSprite;
            }
            else
            {
                selectedButtonImage.color = correctColor;
            }
            Debug.Log("Jawaban benar!");
            
            // Tampilkan pertanyaan berikutnya setelah delay
            StartCoroutine(ShowNextQuestion(2f));
        }
        else
        {
            // Jawaban salah
            if (wrongSprite != null)
            {
                selectedButtonImage.sprite = wrongSprite;
            }
            else
            {
                selectedButtonImage.color = wrongColor;
            }
            Debug.Log("Jawaban salah!");
            
            // Tampilkan jawaban yang benar
            Image correctButtonImage = answerButtons[question.correctAnswerIndex].GetComponent<Image>();
            if (correctSprite != null)
            {
                correctButtonImage.sprite = correctSprite;
            }
            else
            {
                correctButtonImage.color = correctColor;
            }
            
            StartCoroutine(ShowNextQuestion(3f));
        }
    }

    IEnumerator ShowNextQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Langsung tutup panel setelah menjawab
        Debug.Log("Jawaban sudah diterima! Panel ditutup.");
        if (quizPanel != null)
        {
            quizPanel.SetActive(false);
        }
    }

    // Fungsi untuk menambah pertanyaan dari script atau inspector
    public void AddQuestion(string question, List<string> answers, int correctIndex)
    {
        Question newQuestion = new Question
        {
            questionText = question,
            answers = answers,
            correctAnswerIndex = correctIndex
        };
        questions.Add(newQuestion);
    }

    // Fungsi untuk clear semua pertanyaan (berguna untuk reset station)
    public void ClearQuestions()
    {
        questions.Clear();
        currentQuestionIndex = 0;
        isAnswered = false;
    }

    // Fungsi untuk set pertanyaan langsung (mengganti semua yang ada)
    public void SetQuestions(List<Question> newQuestions)
    {
        questions = new List<Question>(newQuestions);
        currentQuestionIndex = 0;
        isAnswered = false;
    }

    // Cek apakah quiz sudah diselesaikan
    public bool IsQuizCompleted()
    {
        return quizCompleted;
    }

    // Reset quiz status (untuk restart)
    public void ResetQuizStatus()
    {
        quizCompleted = false;
        isAnswered = false;
    }
}
