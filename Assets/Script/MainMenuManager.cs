using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Fungsi ini harus public agar bisa dipanggil oleh tombol UI
    public void GoToGameplay()
    {
        // Pastikan nama scene persis sama dengan yang ada di project Anda, termasuk huruf besar/kecil
        SceneManager.LoadScene("Gameplay");
    }

    // Tambahan: Fungsi untuk keluar dari game
    public void QuitGame()
    {
        Debug.Log("Keluar dari game!");
        Application.Quit();
    }
}