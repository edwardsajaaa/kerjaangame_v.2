using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance; // Singleton

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load scene berdasarkan nama
    public void LoadScene(string sceneName)
    {
        Debug.Log($"[SCENE] Loading scene: {sceneName}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    // Load scene berdasarkan index di Build Settings
    public void LoadSceneByIndex(int sceneIndex)
    {
        Debug.Log($"[SCENE] Loading scene at index: {sceneIndex}");
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    // Load main menu
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }

    // Load gameplay scene
    public void LoadGameplay()
    {
        LoadScene("Gameplay");
    }

    // Reload current scene
    public void ReloadScene()
    {
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    // Quit game
    public void QuitGame()
    {
        Debug.Log("[GAME] Quitting application...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

