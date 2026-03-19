using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.25f;
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    float moveInput;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Auto create GroundCheck if missing
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -0.6f, 0);
            groundCheck = gc.transform;
        }
    }

    void Start()
    {
        // Restore position when Continue
        if (GameManager.Instance != null && GameManager.Instance.hasSaved)
        {
            transform.position = GameManager.Instance.lastPlayerPosition;
        }
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        Flip();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // Left - Right
        moveInput =
            (kb.aKey.isPressed || kb.leftArrowKey.isPressed) ? -1 :
            (kb.dKey.isPressed || kb.rightArrowKey.isPressed) ? 1 : 0;

        // Jump
        if (kb.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void Flip()
    {
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void OnDrawGizmos()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
