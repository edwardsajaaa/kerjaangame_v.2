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

    void Start()
    {
        // Pastikan ada 4 button untuk jawaban
        if (answerButtons.Count != 4)
        {
            Debug.LogError("Harus ada 4 tombol jawaban!");
            return;
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

        if (currentQuestionIndex + 1 < questions.Count)
        {
            DisplayQuestion(currentQuestionIndex + 1);
        }
        else
        {
            Debug.Log("Quiz selesai!");
            // Bisa tambah logic lain di sini, misalnya tutup panel
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
}
