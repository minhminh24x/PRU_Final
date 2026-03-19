using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  public float walkSpeed = 5f;
  public float runSpeed = 8f;
  public float jumpForce = 8f;
  public int maxJump = 2;
  public Transform groundCheck;
  public float groundRadius = 0.15f;
  public LayerMask groundLayer;

  Rigidbody2D rb;
  Animator anim;

  PlayerInputActions input;
  Vector2 moveInput;

  bool isGrounded;
  bool isRunning;
  int jumpCount;

  float facing = 1f;
  Vector3 originalScale;

  int attackIndex;
  float comboTimer;
  public float comboResetTime = 0.6f;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    originalScale = transform.localScale;
    input = new PlayerInputActions();
  }

  void OnEnable()
  {
    input.Player.Enable();

    input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    input.Player.Move.canceled += _ => moveInput = Vector2.zero;

    input.Player.Run.performed += _ => isRunning = true;
    input.Player.Run.canceled += _ => isRunning = false;

    input.Player.Jump.performed += _ => Jump();
    input.Player.Attack.performed += _ => Attack();
  }

  void OnDisable()
  {
    input.Player.Disable();
  }

  void Update()
  {
    isGrounded = Physics2D.OverlapCircle(
        groundCheck.position,
        groundRadius,
        groundLayer
    );

    if (isGrounded)
      jumpCount = 0;

    Flip();

    bool isMoving = moveInput.x != 0;
    anim.SetBool("isMoving", isMoving);
    anim.SetBool("isRunning", isRunning && isMoving);

    if (comboTimer > 0)
      comboTimer -= Time.deltaTime;
    else
      attackIndex = 0;
  }

  void FixedUpdate()
  {
    float speed = isRunning ? runSpeed : walkSpeed;
    rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
  }

  void Jump()
  {
    if (jumpCount >= maxJump) return;

    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    jumpCount++;
    anim.SetTrigger("jump");
  }

  void Attack()
  {
    if (comboTimer > 0)
      attackIndex++;
    else
      attackIndex = 1;

    attackIndex = Mathf.Clamp(attackIndex, 1, 3);

    anim.SetInteger("attackIndex", attackIndex);
    anim.SetTrigger("attack");

    comboTimer = comboResetTime;
  }

  void Flip()
  {
    if (moveInput.x > 0 && facing < 0)
    {
      facing = 1f;
      transform.localScale = new Vector3(
          Mathf.Abs(originalScale.x),
          originalScale.y,
          originalScale.z
      );
    }
    else if (moveInput.x < 0 && facing > 0)
    {
      facing = -1f;
      transform.localScale = new Vector3(
          -Mathf.Abs(originalScale.x),
          originalScale.y,
          originalScale.z
      );
    }
  }

  void OnDrawGizmosSelected()
  {
    if (groundCheck == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
  }
}