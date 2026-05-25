using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinishPosManager : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private GameObject successPanel; // Panel selamat (semua soal selesai)
    [SerializeField] private GameObject incompletePanel; // Panel belum selesai
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private TextMeshProUGUI playTimeText; // Optional: tampilkan waktu bermain di finish panel
    
    private bool playerInRange = false;
    private GameObject player;
    private CircleCollider2D rangeCollider;
    
    // Reference untuk mengecek total pertanyaan
    private int totalQuestions = 9;         // Soal normal (tanpa timer) - sumber poin
    [SerializeField] private int totalTimerQuizzes = 3; // Soal enemy (timer) - wajib dijawab semua

    void Start()
    {
        // Cari player di scene
        player = GameObject.FindWithTag("Player");
        
        if (finishPanel == null)
        {
            Debug.LogError("Finish Panel tidak di-assign pada FinishPosManager!");
        }
        
        if (successPanel == null)
        {
            Debug.LogWarning("Success Panel tidak di-assign pada FinishPosManager!");
        }
        
        if (incompletePanel == null)
        {
            Debug.LogWarning("Incomplete Panel tidak di-assign pada FinishPosManager!");
        }
        
        // Buat collider untuk range detection
        rangeCollider = GetComponent<CircleCollider2D>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        rangeCollider.radius = interactionRange;
        rangeCollider.isTrigger = true;
    }

    void Update()
    {
        // Update tidak perlu apa-apa, panel sudah auto trigger
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika objek yang masuk adalah player
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player mencapai pos finish!");
            
            // Aktifkan panel berdasarkan status quiz
            CheckAndShowFinishPanel();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Cek jika player keluar dari range
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player keluar dari pos finish.");
            
            // Nonaktifkan panel ketika player keluar range
            DeactivatePanels();
        }
    }

    void CheckAndShowFinishPanel()
    {
        // Stop & simpan gameplay timer
        if (GameplayTimer.instance != null)
        {
            GameplayTimer.instance.StopAndSaveTime();
        }

        // Ambil data progres
        int currentScore = ScoreManager.instance.GetCurrentScore();
        int enemyCompleted = QuizManager.timerQuizCompletedCount;

        bool allNormalDone = currentScore >= totalQuestions;
        bool allEnemyDone  = enemyCompleted >= totalTimerQuizzes;

        Debug.Log($"[FINISH] Skor normal: {currentScore}/{totalQuestions} | Enemy selesai: {enemyCompleted}/{totalTimerQuizzes}");
        
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
        }

        // Tampilkan waktu bermain di panel finish
        if (playTimeText != null && GameplayTimer.instance != null)
        {
            playTimeText.text = $"Waktu: {GameplayTimer.instance.GetFormattedTime()}";
        }

        // Sukses = semua soal normal benar DAN semua enemy sudah dijawab
        if (allNormalDone && allEnemyDone)
        {
            if (successPanel != null)
            {
                successPanel.SetActive(true);
                if (incompletePanel != null) incompletePanel.SetActive(false);
            }
            Debug.Log($"🏆 SELAMAT! Semua selesai! Skor: {currentScore}/{totalQuestions}, Enemy: {enemyCompleted}/{totalTimerQuizzes}");
        }
        else
        {
            if (incompletePanel != null)
            {
                incompletePanel.SetActive(true);
                if (successPanel != null) successPanel.SetActive(false);
            }

            if (!allNormalDone)
                Debug.Log($"❌ Belum selesai! Skor normal: {currentScore}/{totalQuestions}");
            if (!allEnemyDone)
                Debug.Log($"❌ Belum selesai! Enemy: {enemyCompleted}/{totalTimerQuizzes}");
        }
    }


    // Function untuk deaktifkan semua panel
    public void DeactivatePanels()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
        }
        
        if (successPanel != null)
        {
            successPanel.SetActive(false);
        }
        
        if (incompletePanel != null)
        {
            incompletePanel.SetActive(false);
        }
        
        Debug.Log("Semua panel dinonaktifkan!");
    }

    // Function untuk close panel dari button (bisa dipanggil dari UI)
    public void CloseFinishPanel()
    {
        DeactivatePanels();
    }

    // Optional: visualisasi range di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
