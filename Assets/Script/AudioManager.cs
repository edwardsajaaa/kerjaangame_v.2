using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// AudioManager — Mengelola Background Music per scene
/// 
/// Setup:
/// - Buat Empty GameObject "AudioManager" di scene MainMenu
/// - Tambah script ini
/// - Assign clip musik untuk setiap scene di Inspector
/// - DontDestroyOnLoad agar musik bisa fade antar scene
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Clips — sesuaikan nama scene")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplay1Music;
    [SerializeField] private AudioClip gameplay2Music;
    [SerializeField] private AudioClip gameplay3Music;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip buttonClickSound;  // Suara klik tombol
    [SerializeField] private AudioClip buttonHoverSound;  // Suara hover tombol (opsional)
    [SerializeField] [Range(0f, 1f)] private float uiVolume = 0.8f;

    [Header("Music Settings")]
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.5f;
    [SerializeField] private float fadeDuration = 1.5f; // Durasi fade in/out antar scene

    private AudioSource audioSource;    // Untuk musik background
    private AudioSource audioSourceUI;  // Untuk UI SFX (one-shot)
    private Coroutine fadeCoroutine;

    void Awake()
    {
        // Singleton — DontDestroyOnLoad agar musik tidak putus saat ganti scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop        = true;
        audioSource.playOnAwake = false;
        audioSource.volume      = musicVolume;

        // AudioSource terpisah untuk UI SFX
        audioSourceUI               = gameObject.AddComponent<AudioSource>();
        audioSourceUI.loop          = false;
        audioSourceUI.playOnAwake   = false;
        audioSourceUI.volume        = uiVolume;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Putar musik scene pertama
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Dipanggil otomatis setiap kali scene baru di-load
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    /// <summary>
    /// Pilih dan putar musik berdasarkan nama scene
    /// </summary>
    void PlayMusicForScene(string sceneName)
    {
        AudioClip targetClip = sceneName switch
        {
            "MainMenu"   => mainMenuMusic,
            "Gameplay1"  => gameplay1Music,
            "Gameplay2"  => gameplay2Music,
            "Gameplay3"  => gameplay3Music,
            _            => null
        };

        if (targetClip == null)
        {
            Debug.LogWarning($"[AUDIO] Tidak ada musik untuk scene: {sceneName}");
            return;
        }

        // Jika musik sama, jangan restart
        if (audioSource.clip == targetClip && audioSource.isPlaying)
        {
            Debug.Log($"[AUDIO] Musik sudah berjalan untuk scene: {sceneName}");
            return;
        }

        // Fade out → ganti clip → fade in
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeToNewClip(targetClip));

        Debug.Log($"[AUDIO] Memutar musik untuk scene: {sceneName}");
    }

    IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // Fade out musik lama
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            float t = 0f;
            while (t < fadeDuration / 2f)
            {
                t += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / (fadeDuration / 2f));
                yield return null;
            }
        }

        // Ganti clip dan play
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in musik baru
        float t2 = 0f;
        while (t2 < fadeDuration / 2f)
        {
            t2 += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, t2 / (fadeDuration / 2f));
            yield return null;
        }

        audioSource.volume = musicVolume;
    }

    // ===== Public Methods — Music =====

    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    public float GetVolume() => musicVolume;

    public void PauseMusic()  => audioSource.Pause();
    public void ResumeMusic() => audioSource.UnPause();
    public void StopMusic()   => audioSource.Stop();

    // ===== Static Methods — UI Button SFX =====

    /// <summary>
    /// Putar suara klik tombol.
    /// Assign ke Button OnClick() → AudioManager → PlayButtonClick()
    /// ATAU panggil dari script: AudioManager.PlayButtonClick()
    /// </summary>
    public void PlayButtonClick()
    {
        if (buttonClickSound != null && audioSourceUI != null)
            audioSourceUI.PlayOneShot(buttonClickSound, uiVolume);
    }

    /// <summary>
    /// Putar suara hover tombol (opsional).
    /// Assign ke EventTrigger PointerEnter.
    /// </summary>
    public void PlayButtonHover()
    {
        if (buttonHoverSound != null && audioSourceUI != null)
            audioSourceUI.PlayOneShot(buttonHoverSound, uiVolume * 0.6f);
    }

    /// <summary>
    /// Static shortcut — bisa dipanggil dari mana saja tanpa cari instance
    /// </summary>
    public static void Click()
    {
        if (instance != null) instance.PlayButtonClick();
    }

    public static void Hover()
    {
        if (instance != null) instance.PlayButtonHover();
    }
}
