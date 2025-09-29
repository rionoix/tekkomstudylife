using UnityEngine;
using UnityEngine.InputSystem; // penting untuk InputValue

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector2 moveInput;          // arah gerakan (X,Y)
    private Rigidbody2D rb;             // physics
    private Animator animator;          // untuk animasi
    private SpriteRenderer spriteRenderer; // untuk flip sprite

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ===== DIPANGGIL OTOMATIS OLEH PlayerInput (Send Messages) =====
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // ===== PERGERAKAN PHYSICS (tiap fixed frame) =====
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // ===== UPDATE ANIMASI & FLIP SPRITE =====
    void Update()
    {
        // Kirim nilai ke Animator
        animator.SetFloat("Horizontal", moveInput.x);
        animator.SetFloat("Vertical", moveInput.y);
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        // Flip sprite (mirror kiri/kanan)
        if (moveInput.x > 0.01f)
            spriteRenderer.flipX = false; // menghadap kanan
        else if (moveInput.x < -0.01f)
            spriteRenderer.flipX = true;  // menghadap kiri

        // Debug untuk cek input
        // Debug.Log("Move: " + moveInput + " | Speed: " + moveInput.sqrMagnitude);
    }
}
