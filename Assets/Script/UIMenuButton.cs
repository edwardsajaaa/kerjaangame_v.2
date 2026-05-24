using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuButton : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        // Setup button listeners
        if (playButton != null)
        {
            playButton.onClick.AddListener(() => OnPlayButtonClicked());
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(() => OnQuitButtonClicked());
        }
    }

    // Ketika tombol Play diklik
    void OnPlayButtonClicked()
    {
        Debug.Log("[UI] Play button clicked!");
        SceneManager.instance.LoadGameplay();
    }

    // Ketika tombol Quit diklik
    void OnQuitButtonClicked()
    {
        Debug.Log("[UI] Quit button clicked!");
        SceneManager.instance.QuitGame();
    }
}
