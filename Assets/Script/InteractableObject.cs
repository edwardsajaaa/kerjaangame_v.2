using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private GameObject panelToActivate;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    private bool playerInRange = false;
    private GameObject player;
    private CircleCollider2D rangeCollider;

    // ===== Persistent Timer (berjalan terus walau panel nonaktif) =====
    private bool isPersistentTimerActive = false;
    private float timerRemaining = 0f;
    private float timerDuration = 20f;
    private QuizManager cachedQuizManager;

    void Start()
    {
        // Cari player di scene
        player = GameObject.FindWithTag("Player");
        
        if (panelToActivate == null)
        {
            Debug.LogError("Panel tidak di-assign pada InteractableObject!");
        }
        
        // Buat collider untuk range detection
        rangeCollider = GetComponent<CircleCollider2D>();
        if (rangeCollider == null)
        {
            rangeCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        rangeCollider.radius = interactionRange;
        rangeCollider.isTrigger = true;

        // Cache QuizManager reference
        if (panelToActivate != null)
        {
            cachedQuizManager = panelToActivate.GetComponent<QuizManager>();
        }
    }

    void Update()
    {
        // ===== Persistent Timer Logic =====
        if (!isPersistentTimerActive) return;

        // Cek apakah quiz sudah dijawab → stop timer
        if (cachedQuizManager != null && cachedQuizManager.IsQuizCompleted())
        {
            StopPersistentTimer();
            Debug.Log("✅ Quiz dijawab! Timer dihentikan.");
            return;
        }

        // Countdown
        timerRemaining -= Time.deltaTime;

        // Update display jika panel sedang aktif (terlihat player)
        if (cachedQuizManager != null && panelToActivate != null && panelToActivate.activeInHierarchy)
        {
            cachedQuizManager.UpdateTimerDisplay(timerRemaining);
        }

        // Timer habis!
        if (timerRemaining <= 0f)
        {
            OnPersistentTimerExpired();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Cek jika objek yang masuk adalah player
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player mendekati! Panel ditampilkan otomatis.");
            
            // Aktifkan panel secara otomatis
            Interact();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // Cek jika player keluar dari range
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player keluar dari range.");
            
            // Nonaktifkan panel ketika player keluar range
            DeactivatePanel();
        }
    }

    void Interact()
    {
        if (panelToActivate == null) return;

        // Cek apakah panel punya QuizManager dan sudah diselesaikan
        if (cachedQuizManager != null && cachedQuizManager.IsQuizCompleted())
        {
            Debug.Log("Quiz sudah diselesaikan! Panel tidak ditampilkan lagi.");
            return; // Jangan tampilkan panel
        }

        // CEK: apakah quiz ini punya timer?
        bool hasTimer = false;
        float questionTimerDuration = 20f;
        if (cachedQuizManager != null && cachedQuizManager.GetQuestions().Count > 0)
        {
            int currentIndex = cachedQuizManager.GetCurrentQuestionIndex();
            QuizManager.Question currentQuestion = cachedQuizManager.GetQuestion(currentIndex);
            if (currentQuestion != null)
            {
                hasTimer = currentQuestion.useTimer;
                questionTimerDuration = currentQuestion.timerDuration;
            }
        }
        
        // Kalau TIDAK ada timer, set spawn point
        if (!hasTimer)
        {
            RespawnPoint.lastSpawnPosition = transform.position;
            Debug.Log($"🚩 Spawn point updated: {transform.position}");
        }
        else
        {
            Debug.Log("⏱️ Timer quiz - spawn point TIDAK diupdate");
        }

        // Aktifkan panel
        panelToActivate.SetActive(true);
        Debug.Log("Panel diaktifkan!");

        // Start persistent timer jika quiz pakai timer DAN belum jalan
        if (hasTimer && !isPersistentTimerActive)
        {
            StartPersistentTimer(questionTimerDuration);
        }
    }

    // ===== Persistent Timer Methods =====

    void StartPersistentTimer(float duration)
    {
        timerDuration = duration;
        timerRemaining = duration;
        isPersistentTimerActive = true;
        Debug.Log($"⏱️ Persistent timer dimulai: {duration} detik");
    }

    void StopPersistentTimer()
    {
        isPersistentTimerActive = false;

        // Sembunyikan timer display
        if (cachedQuizManager != null)
        {
            cachedQuizManager.HideTimerDisplay();
        }
    }

    void OnPersistentTimerExpired()
    {
        Debug.Log("⏰ Timer habis!");

        // Kurangi health
        if (HealthManager.instance != null)
        {
            HealthManager.instance.DecreaseHealth(1);
            Debug.Log("⏰ Timer quiz habis! -1 HP");

            // Cek apakah player masih hidup
            if (!HealthManager.instance.IsAlive())
            {
                StopPersistentTimer();
                Debug.Log("💀 Player mati! Timer dihentikan.");
                return;
            }
        }

        // Reset timer dari awal
        timerRemaining = timerDuration;
        Debug.Log($"⏰ Timer reset ke {timerDuration} detik. Masih berjalan...");
    }

    // Function untuk deaktifkan panel (bisa dipanggil dari button di panel)
    public void DeactivatePanel()
    {
        if (panelToActivate != null)
        {
            panelToActivate.SetActive(false);
            Debug.Log("Panel dinonaktifkan!");
        }
        // CATATAN: Timer TIDAK dihentikan di sini!
        // Timer tetap berjalan walau panel nonaktif
    }

    // Optional: visualisasi range di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
