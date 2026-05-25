using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Fungsi ini harus public agar bisa dipanggil oleh tombol UI
    public void GoToGameplay()
    {
        // Pastikan nama scene persis sama dengan yang ada di project Anda, termasuk huruf besar/kecil
        SceneManager.LoadScene("Gameplay1");
    }

    // Fungsi untuk kembali ke MainMenu
    public void GoToMainMenu()
    {
        Debug.Log("Kembali ke MainMenu...");
        SceneManager.LoadScene("MainMenu");
    }

    // Fungsi untuk ke Gameplay_2
    public void GoToGameplay2()
    {
        Debug.Log("Menuju Gameplay2...");
        SceneManager.LoadScene("Gameplay2");
    }

    // Tambahan: Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Debug.Log("Keluar dari game!");
        Application.Quit();
    }
}