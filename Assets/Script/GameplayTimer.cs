using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// GameplayTimer - Menghitung berapa lama player bermain
/// Waktu & skor tersimpan di PlayerPrefs sebagai leaderboard entries
/// 
/// Setup:
/// - Buat Empty GameObject "GameplayTimer"
/// - Tambah script ini
/// - Assign TextMeshProUGUI untuk display waktu
/// </summary>
public class GameplayTimer : MonoBehaviour
{
    public static GameplayTimer instance;

    [SerializeField] private TextMeshProUGUI timerDisplay; // UI Text untuk tampilkan waktu
    
    private float elapsedTime = 0f; // Waktu bermain dalam detik
    private bool isRunning = true; // Timer berjalan atau tidak

    // Key untuk PlayerPrefs
    private const string LEADERBOARD_KEY = "LeaderboardData";
    private const int MAX_ENTRIES = 10; // Maksimal 10 entry di leaderboard

    // Data structure untuk leaderboard
    [System.Serializable]
    public class LeaderboardEntry
    {
        public float playTime;    // Waktu bermain (detik)
        public int score;         // Skor (jawaban benar)
        public string date;       // Tanggal bermain
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

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
        elapsedTime = 0f;
        isRunning = true;
        UpdateTimerDisplay();
        Debug.Log("[TIMER] Gameplay timer dimulai!");
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Update tampilan timer di UI (format M:SS)
    /// </summary>
    void UpdateTimerDisplay()
    {
        if (timerDisplay != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerDisplay.text = $"{minutes}:{seconds:D2}";
        }
    }

    /// <summary>
    /// Stop timer dan simpan waktu bermain ke leaderboard.
    /// Panggil saat player finish atau game over.
    /// </summary>
    public void StopAndSaveTime()
    {
        if (!isRunning) return;

        isRunning = false;

        // Ambil skor dari ScoreManager
        int score = 0;
        if (ScoreManager.instance != null)
        {
            score = ScoreManager.instance.GetCurrentScore();
        }

        // Buat entry baru
        LeaderboardEntry newEntry = new LeaderboardEntry
        {
            playTime = elapsedTime,
            score = score,
            date = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm")
        };

        // Simpan ke leaderboard
        SaveToLeaderboard(newEntry);

        Debug.Log($"⏱️ Timer berhenti! Waktu: {FormatTime(elapsedTime)} | Skor: {score}");
    }

    /// <summary>
    /// Simpan entry ke leaderboard (sorted by score desc, lalu time asc)
    /// </summary>
    void SaveToLeaderboard(LeaderboardEntry newEntry)
    {
        LeaderboardData data = LoadLeaderboardData();
        data.entries.Add(newEntry);

        // Sort: skor tertinggi dulu, jika sama maka waktu tercepat
        data.entries.Sort((a, b) =>
        {
            int scoreCompare = b.score.CompareTo(a.score); // Skor tinggi di atas
            if (scoreCompare != 0) return scoreCompare;
            return a.playTime.CompareTo(b.playTime); // Waktu cepat di atas
        });

        // Batasi jumlah entry
        if (data.entries.Count > MAX_ENTRIES)
        {
            data.entries.RemoveRange(MAX_ENTRIES, data.entries.Count - MAX_ENTRIES);
        }

        // Simpan ke PlayerPrefs sebagai JSON
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();

        Debug.Log($"💾 Leaderboard disimpan! Total entries: {data.entries.Count}");
    }

    // ===== Static Methods (bisa dipanggil dari scene mana saja) =====

    /// <summary>
    /// Load leaderboard data dari PlayerPrefs
    /// Bisa dipanggil dari MainMenu tanpa perlu instance
    /// </summary>
    public static LeaderboardData LoadLeaderboardData()
    {
        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "");
        
        if (string.IsNullOrEmpty(json))
        {
            return new LeaderboardData();
        }

        try
        {
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        catch
        {
            Debug.LogWarning("⚠️ Gagal load leaderboard data, membuat baru.");
            return new LeaderboardData();
        }
    }

    /// <summary>
    /// Hapus semua data leaderboard
    /// </summary>
    public static void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        PlayerPrefs.Save();
        Debug.Log("🗑️ Leaderboard data dihapus!");
    }

    // ===== Getter Functions =====

    /// <summary>
    /// Ambil waktu bermain saat ini (dalam detik)
    /// </summary>
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    /// <summary>
    /// Ambil waktu bermain dalam format string "M:SS"
    /// </summary>
    public string GetFormattedTime()
    {
        return FormatTime(elapsedTime);
    }

    /// <summary>
    /// Cek apakah timer sedang berjalan
    /// </summary>
    public bool IsRunning()
    {
        return isRunning;
    }

    /// <summary>
    /// Pause timer
    /// </summary>
    public void PauseTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// Resume timer
    /// </summary>
    public void ResumeTimer()
    {
        isRunning = true;
    }

    // ===== Utility =====

    /// <summary>
    /// Format detik menjadi "M:SS"
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return $"{minutes}:{seconds:D2}";
    }
}
