using UnityEngine;

/// <summary>
/// QuitButton — Script untuk tombol keluar aplikasi
/// 
/// Setup (Tanpa Konfirmasi):
/// - Attach script ini ke Button "Keluar"
/// - Di Button OnClick() → QuitButton → QuitGame()
///
/// Setup (Dengan Konfirmasi Panel):
/// - Assign confirmPanel di Inspector
/// - OnClick tombol "Keluar" → QuitButton → ShowConfirmPanel()
/// - OnClick tombol "Ya"     → QuitButton → QuitGame()
/// - OnClick tombol "Tidak"  → QuitButton → HideConfirmPanel()
/// </summary>
public class QuitButton : MonoBehaviour
{
    [Header("Optional — Panel Konfirmasi")]
    [SerializeField] private GameObject confirmPanel; // Panel "Yakin ingin keluar?"

    /// <summary>
    /// Tampilkan panel konfirmasi sebelum keluar.
    /// Assign ke OnClick() tombol "Keluar" jika pakai konfirmasi.
    /// </summary>
    public void ShowConfirmPanel()
    {
        AudioManager.Click();

        if (confirmPanel != null)
            confirmPanel.SetActive(true);
        else
            QuitGame(); // Kalau tidak ada panel, langsung keluar
    }

    /// <summary>
    /// Sembunyikan panel konfirmasi (tombol "Tidak" / "Batal").
    /// </summary>
    public void HideConfirmPanel()
    {
        AudioManager.Click();

        if (confirmPanel != null)
            confirmPanel.SetActive(false);
    }

    /// <summary>
    /// Keluar dari aplikasi.
    /// Assign ke OnClick() tombol "Keluar" (tanpa konfirmasi)
    /// atau tombol "Ya" di panel konfirmasi.
    /// </summary>
    public void QuitGame()
    {
        AudioManager.Click();

        Debug.Log("🚪 Keluar dari aplikasi...");

#if UNITY_EDITOR
        // Saat di Unity Editor → stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Saat build → keluar aplikasi
        Application.Quit();
#endif
    }
}
