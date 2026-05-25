using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Movement Variables
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 2f;

    // Ground Check
    [SerializeField] private float groundDist = 0.5f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;

    // Components
    private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    private float currentSpeed;
    private bool isRunning;
    private bool isJumping;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D tidak ditemukan pada player!");
        }
        
        if (animator == null)
        {
            Debug.LogError("Animator tidak ditemukan pada player!");
        }
        
        // Set Rigidbody untuk smooth movement
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Handle input saja di Update
        HandleMovementInput();
        HandleRunInput();
        HandleJumpInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        // Physics calculation di FixedUpdate
        CheckGround();
        ApplyDrag();
        ApplyMovement();
    }

    void CheckGround()
    {
        // Method 1: Gunakan OverlapCircle (lebih reliable)
        Vector2 checkPos = new Vector2(transform.position.x, transform.position.y - groundDist);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPos, groundCheckRadius, groundLayer);
        isGrounded = colliders.Length > 0;
        
        // Debug
        Debug.Log($"IsGrounded: {isGrounded} | Colliders detected: {colliders.Length}");
    }

    void HandleMovementInput()
    {
        moveInput = 0f;

        // Get input for movement
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }

        // Flip sprite based on direction
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void ApplyMovement()
    {
        // Determine speed (walking or running)
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Apply movement
        if (moveInput != 0)
        {
            rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
        }
        else
        {
            // Stop horizontal movement
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void HandleRunInput()
    {
        // Press Shift to run
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Jump
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        // Set isJumping false ketika sudah grounded dan tidak ada upward velocity
        if (isGrounded && rb.velocity.y <= 0 && isJumping)
        {
            isJumping = false;
        }
    }

    void ApplyDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    // Optional: Function to check if player is grounded (dapat digunakan untuk animasi)
    public bool IsGrounded()
    {
        return isGrounded;
    }

    // Optional: Function to check if player is running
    public bool IsRunning()
    {
        return isRunning;
    }

    // Animation Functions
    void UpdateAnimations()
    {
        if (animator == null) return;

        // Set parameter running
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);

        // Jika pemain bergerak, mainkan animasi Walk
        if (moveInput != 0)
        {
            SetWalk();
        }
        else
        {
            // Jika pemain tidak bergerak, mainkan animasi Idle
            SetIdle();
        }
    }

    // Function untuk set animasi Idle
    void SetIdle()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    // Function untuk set animasi Walk
    void SetWalk()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    // Respawn ke spawn point terakhir
    public void Respawn()
    {
        transform.position = RespawnPoint.lastSpawnPosition;
        rb.velocity = Vector2.zero; // Reset velocity
        isJumping = false;
        Debug.Log($"↩️ Player respawned to: {RespawnPoint.lastSpawnPosition}");
    }

    // Visualisasi ground check untuk debugging
    void OnDrawGizmos()
    {
        // Draw OverlapCircle visualization
        Vector2 checkPos = new Vector2(transform.position.x, transform.position.y - groundDist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
