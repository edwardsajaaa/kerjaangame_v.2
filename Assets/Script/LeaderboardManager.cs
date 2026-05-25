using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// LeaderboardManager - Menampilkan leaderboard di Main Menu
/// Membaca data dari PlayerPrefs yang disimpan oleh GameplayTimer
/// 
/// Setup:
/// 1. Taruh script ini di panel LeaderBoard di scene MainMenu
/// 2. Assign leaderboardContent (parent dari entry rows)
/// 3. Assign entryPrefab (prefab untuk setiap row)
/// 
/// Entry Prefab harus punya 4 child TextMeshProUGUI:
/// - RankText (contoh: "#1")
/// - ScoreText (contoh: "9/9")
/// - TimeText (contoh: "2:45")
/// - DateText (contoh: "25/05/2026")
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform leaderboardContent; // Parent untuk entry rows
    [SerializeField] private GameObject entryPrefab; // Prefab untuk setiap baris leaderboard

    [Header("Jika tidak pakai prefab, assign langsung")]
    [SerializeField] private List<TextMeshProUGUI> rankTexts;  // Text untuk rank (#1, #2, dst)
    [SerializeField] private List<TextMeshProUGUI> scoreTexts; // Text untuk skor
    [SerializeField] private List<TextMeshProUGUI> timeTexts;  // Text untuk waktu
    [SerializeField] private List<TextMeshProUGUI> dateTexts;  // Text untuk tanggal

    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI noDataText; // Text "Belum ada data"
    [SerializeField] private int maxDisplayEntries = 10; // Berapa entry yang ditampilkan

    void OnEnable()
    {
        // Refresh leaderboard setiap kali panel dibuka
        RefreshLeaderboard();
    }

    /// <summary>
    /// Muat dan tampilkan data leaderboard
    /// </summary>
    public void RefreshLeaderboard()
    {
        // Load data dari PlayerPrefs
        GameplayTimer.LeaderboardData data = GameplayTimer.LoadLeaderboardData();

        if (entryPrefab != null && leaderboardContent != null)
        {
            // === MODE PREFAB: Spawn entry rows dinamis ===
            DisplayWithPrefab(data);
        }
        else
        {
            // === MODE MANUAL: Assign text langsung ===
            DisplayWithManualTexts(data);
        }

        // Tampilkan/sembunyikan "Belum ada data"
        if (noDataText != null)
        {
            noDataText.gameObject.SetActive(data.entries.Count == 0);
        }

        Debug.Log($"[LEADERBOARD] Loaded {data.entries.Count} entries");
    }

    /// <summary>
    /// Tampilkan leaderboard menggunakan prefab (dinamis)
    /// </summary>
    void DisplayWithPrefab(GameplayTimer.LeaderboardData data)
    {
        // Hapus entry lama
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Buat entry baru
        int count = Mathf.Min(data.entries.Count, maxDisplayEntries);
        for (int i = 0; i < count; i++)
        {
            GameplayTimer.LeaderboardEntry entry = data.entries[i];
            GameObject row = Instantiate(entryPrefab, leaderboardContent);
            row.SetActive(true);

            // Cari TextMeshProUGUI children
            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            
            // Assign berdasarkan urutan (Rank, Score, Time, Date)
            if (texts.Length >= 1) texts[0].text = $"#{i + 1}";
            if (texts.Length >= 2) texts[1].text = $"{entry.score}";
            if (texts.Length >= 3) texts[2].text = GameplayTimer.FormatTime(entry.playTime);
            if (texts.Length >= 4) texts[3].text = entry.date;
        }
    }

    /// <summary>
    /// Tampilkan leaderboard menggunakan text yang sudah di-assign manual
    /// </summary>
    void DisplayWithManualTexts(GameplayTimer.LeaderboardData data)
    {
        int count = Mathf.Min(data.entries.Count, maxDisplayEntries);

        for (int i = 0; i < maxDisplayEntries; i++)
        {
            if (i < count)
            {
                // Ada data → tampilkan
                GameplayTimer.LeaderboardEntry entry = data.entries[i];

                if (i < rankTexts.Count && rankTexts[i] != null)
                    rankTexts[i].text = $"#{i + 1}";

                if (i < scoreTexts.Count && scoreTexts[i] != null)
                    scoreTexts[i].text = $"{entry.score}";

                if (i < timeTexts.Count && timeTexts[i] != null)
                    timeTexts[i].text = GameplayTimer.FormatTime(entry.playTime);

                if (i < dateTexts.Count && dateTexts[i] != null)
                    dateTexts[i].text = entry.date;
            }
            else
            {
                // Tidak ada data → kosongkan
                if (i < rankTexts.Count && rankTexts[i] != null)
                    rankTexts[i].text = "";

                if (i < scoreTexts.Count && scoreTexts[i] != null)
                    scoreTexts[i].text = "";

                if (i < timeTexts.Count && timeTexts[i] != null)
                    timeTexts[i].text = "";

                if (i < dateTexts.Count && dateTexts[i] != null)
                    dateTexts[i].text = "";
            }
        }
    }

    /// <summary>
    /// Hapus semua data leaderboard (untuk tombol reset)
    /// </summary>
    public void ClearAllData()
    {
        GameplayTimer.ClearLeaderboard();
        RefreshLeaderboard();
        Debug.Log("🗑️ Leaderboard data dihapus!");
    }
}
