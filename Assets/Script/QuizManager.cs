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
        public bool useTimer = false; // Optional: gunakan timer untuk soal ini?
        public float timerDuration = 20f; // Durasi timer dalam detik
    }

    [SerializeField] private List<Question> questions = new List<Question>();
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private List<Button> answerButtons = new List<Button>();
    [SerializeField] private GameObject quizPanel; // Panel yang akan ditutup saat selesai
    [SerializeField] private TextMeshProUGUI timerText; // Display timer (diupdate oleh InteractableObject)
    
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

    // Static counter: berapa quiz timer (enemy) yang sudah dijawab
    public static int timerQuizCompletedCount = 0;

    void Start()
    {
        // Reset counter enemy quiz saat scene baru
        timerQuizCompletedCount = 0;

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

        // Timer display dikelola oleh InteractableObject
        // Tampilkan timer text jika soal pakai timer
        if (question.useTimer)
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
            }
        }
        else
        {
            HideTimerDisplay();
        }
    }

    void OnAnswerSelected(int selectedIndex)
    {
        if (isAnswered) return;

        isAnswered = true;
        quizCompleted = true; // Quiz sudah diselesaikan → InteractableObject akan stop timer
        Question question = questions[currentQuestionIndex];
        Image selectedButtonImage = answerButtons[selectedIndex].GetComponent<Image>();

        if (selectedIndex == question.correctAnswerIndex)
        {
            // ========== JAWABAN BENAR ==========
            if (correctSprite != null)
            {
                selectedButtonImage.sprite = correctSprite;
            }
            else
            {
                selectedButtonImage.color = correctColor;
            }
            Debug.Log("✓ Jawaban benar!");
            PlayerSoundController.PlayCorrectSound(); // SFX jawaban benar
            
            if (!question.useTimer)
            {
                // Quiz TANPA timer → tambah poin
                int pointsToAdd = ScoreManager.instance.GetPointsPerQuestion();
                ScoreManager.instance.AddPoints(pointsToAdd);
                Debug.Log($"⭐ Poin ditambah! (quiz normal)");
            }
            else
            {
                // Quiz TIMER (enemy) → tidak tambah poin, tapi catat sudah dijawab
                timerQuizCompletedCount++;
                Debug.Log($"⚔️ Enemy quiz selesai! ({timerQuizCompletedCount}/3)");
            }
            
            // Tampilkan pertanyaan berikutnya setelah delay
            StartCoroutine(ShowNextQuestion(2f));
        }
        else
        {
            // ========== JAWABAN SALAH ==========
            if (wrongSprite != null)
            {
                selectedButtonImage.sprite = wrongSprite;
            }
            else
            {
                selectedButtonImage.color = wrongColor;
            }
            Debug.Log("✗ Jawaban salah!");
            PlayerSoundController.PlayWrongSound(); // SFX jawaban salah
            
            // Jika quiz pakai TIMER → langsung kurangi health 1
            if (question.useTimer)
            {
                if (HealthManager.instance != null)
                {
                    HealthManager.instance.DecreaseHealth(1);
                    Debug.Log("💔 Salah di quiz timer! -1 HP");
                }
            }
            
            // Tidak tambah poin untuk jawaban salah
            
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

    // ===== Timer Display Methods (dipanggil oleh InteractableObject) =====

    /// <summary>
    /// Update tampilan timer. Dipanggil oleh InteractableObject setiap frame saat panel aktif.
    /// </summary>
    public void UpdateTimerDisplay(float remaining)
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.Max(0, remaining).ToString("F1");

            // Change color berdasarkan waktu tersisa
            if (remaining <= 5f)
            {
                timerText.color = Color.red; // Merah saat 5 detik tersisa
            }
            else if (remaining <= 10f)
            {
                timerText.color = Color.yellow; // Kuning saat 10 detik
            }
            else
            {
                timerText.color = Color.white; // Putih normal
            }
        }
    }

    /// <summary>
    /// Sembunyikan timer display.
    /// </summary>
    public void HideTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // ===== Utility Methods =====

    // Fungsi untuk menambah pertanyaan dari script atau inspector
    public void AddQuestion(string question, List<string> answers, int correctIndex, bool useTimer = false, float timerDuration = 20f)
    {
        Question newQuestion = new Question
        {
            questionText = question,
            answers = answers,
            correctAnswerIndex = correctIndex,
            useTimer = useTimer,
            timerDuration = timerDuration
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

    // Getter untuk current question index
    public int GetCurrentQuestionIndex()
    {
        return currentQuestionIndex;
    }

    // Getter untuk questions list
    public List<Question> GetQuestions()
    {
        return questions;
    }

    // Getter untuk specific question
    public Question GetQuestion(int index)
    {
        if (index >= 0 && index < questions.Count)
        {
            return questions[index];
        }
        return null;
    }
}
