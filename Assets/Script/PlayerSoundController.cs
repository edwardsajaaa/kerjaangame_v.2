using UnityEngine;

/// <summary>
/// PlayerSoundController — Mengelola Sound Effect karakter
/// 
/// Setup:
/// - Tambah script ini ke GameObject Player
/// - Assign AudioClip di Inspector untuk setiap SFX
/// - Script ini butuh 2 AudioSource di Player:
///   audioSourceMovement (untuk walk loop) dan audioSourceOneShot (untuk jump, benar, salah)
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class PlayerSoundController : MonoBehaviour
{
    [Header("SFX Clips")]
    [SerializeField] private AudioClip walkSound;      // Suara langkah kaki (loop)
    [SerializeField] private AudioClip runSound;       // Suara lari (loop) — opsional, bisa sama dengan walk
    [SerializeField] private AudioClip jumpSound;      // Suara melompat (one-shot)
    [SerializeField] private AudioClip landSound;      // Suara mendarat (one-shot)
    [SerializeField] private AudioClip correctSound;   // Suara jawaban benar (one-shot)
    [SerializeField] private AudioClip wrongSound;     // Suara jawaban salah (one-shot)
    [SerializeField] private AudioClip splashSound;    // Suara kena air (one-shot)

    [Header("Volume")]
    [SerializeField] [Range(0f, 1f)] private float walkVolume  = 0.4f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume   = 0.8f;

    // AudioSource terpisah: satu untuk loop (walk), satu untuk one-shot
    private AudioSource audioLoop;      // Untuk walk/run (loop)
    private AudioSource audioOneShot;   // Untuk jump, benar, salah (one-shot)

    private PlayerMovement playerMovement;
    private bool wasGrounded = true;
    private bool wasMoving = false;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Buat 2 AudioSource di Player
        AudioSource[] sources = GetComponents<AudioSource>();

        if (sources.Length >= 2)
        {
            audioLoop     = sources[0];
            audioOneShot  = sources[1];
        }
        else if (sources.Length == 1)
        {
            audioLoop    = sources[0];
            audioOneShot = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioLoop    = gameObject.AddComponent<AudioSource>();
            audioOneShot = gameObject.AddComponent<AudioSource>();
        }

        // Setup AudioSource loop (untuk walk)
        audioLoop.loop         = true;
        audioLoop.playOnAwake  = false;
        audioLoop.volume       = walkVolume;
        audioLoop.spatialBlend = 0f; // 2D sound

        // Setup AudioSource one-shot
        audioOneShot.loop         = false;
        audioOneShot.playOnAwake  = false;
        audioOneShot.volume       = sfxVolume;
        audioOneShot.spatialBlend = 0f;
    }

    void Update()
    {
        HandleWalkSound();
        HandleLandSound();
    }

    // =============================================
    // Walk Sound — loop saat bergerak di tanah
    // =============================================
    void HandleWalkSound()
    {
        bool isGrounded = playerMovement.IsGrounded();
        bool isRunning  = playerMovement.IsRunning();

        // Cek apakah player bergerak (horizontal input)
        bool isMoving = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > 0.1f;

        if (isGrounded && isMoving)
        {
            // Pilih clip walk atau run
            AudioClip targetClip = (isRunning && runSound != null) ? runSound : walkSound;

            if (targetClip != null)
            {
                // Ganti clip jika berbeda (misal dari walk ke run)
                if (audioLoop.clip != targetClip)
                {
                    audioLoop.clip = targetClip;
                    audioLoop.Play();
                }
                else if (!audioLoop.isPlaying)
                {
                    audioLoop.Play();
                }
            }
        }
        else
        {
            // Berhenti atau di udara → stop walk sound
            if (audioLoop.isPlaying)
            {
                audioLoop.Stop();
            }
        }

        wasMoving = isMoving;
    }

    // =============================================
    // Land Sound — saat mendarat setelah lompat
    // =============================================
    void HandleLandSound()
    {
        bool isGrounded = playerMovement.IsGrounded();

        // Baru saja mendarat
        if (!wasGrounded && isGrounded)
        {
            PlayOneShot(landSound);
        }

        wasGrounded = isGrounded;
    }

    // =============================================
    // Public Methods — dipanggil dari script lain
    // =============================================

    /// <summary>
    /// Putar suara lompat. Dipanggil dari PlayerMovement saat jump.
    /// </summary>
    public void PlayJumpSound()
    {
        PlayOneShot(jumpSound);
    }

    /// <summary>
    /// Putar suara jawaban benar. Dipanggil dari QuizManager.
    /// </summary>
    public static void PlayCorrectSound()
    {
        PlayerSoundController controller = FindObjectOfType<PlayerSoundController>();
        if (controller != null) controller.PlayOneShot(controller.correctSound);
    }

    /// <summary>
    /// Putar suara jawaban salah. Dipanggil dari QuizManager.
    /// </summary>
    public static void PlayWrongSound()
    {
        PlayerSoundController controller = FindObjectOfType<PlayerSoundController>();
        if (controller != null) controller.PlayOneShot(controller.wrongSound);
    }

    /// <summary>
    /// Putar suara kena air. Dipanggil dari WaterDamage.
    /// </summary>
    public static void PlaySplashSound()
    {
        PlayerSoundController controller = FindObjectOfType<PlayerSoundController>();
        if (controller != null) controller.PlayOneShot(controller.splashSound);
    }

    void PlayOneShot(AudioClip clip)
    {
        if (clip != null && audioOneShot != null)
        {
            audioOneShot.PlayOneShot(clip, sfxVolume);
        }
    }
}
