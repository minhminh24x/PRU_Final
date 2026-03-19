using UnityEngine;

public class BossAI : MonoBehaviour
{
  [Header("Target")]
  public Transform player;
  public Transform statuePoint;
  public Transform attackPoint;

  [Header("Move")]
  public float flySpeed = 3f;
  public float returnDistance = 0.1f;

  [Header("Attack")]
  public float attackRange = 1.5f;
  public float attackCooldown = 2f;

  bool isAggro;
  float lastAttackTime;

  Animator anim;
  Rigidbody2D rb;

  Vector3 originalScale;

  void Start()
  {
    anim = GetComponent<Animator>();

    rb = GetComponent<Rigidbody2D>();
    if (rb == null)
      rb = gameObject.AddComponent<Rigidbody2D>();

    rb.bodyType = RigidbodyType2D.Kinematic;
    rb.gravityScale = 0;

    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
      col.isTrigger = true;

    // ✅ LƯU SCALE GỐC (QUAN TRỌNG)
    originalScale = transform.localScale;

    if (statuePoint != null)
      transform.position = statuePoint.position;
  }

  void Update()
  {
    if (isAggro)
      HandleAggro();
    else
      ReturnToStatue();
  }

  void HandleAggro()
  {
    if (player == null) return;

    Face(player.position.x);

    float dist = Mathf.Abs(player.position.x - transform.position.x);

    if (dist <= attackRange && Time.time >= lastAttackTime + attackCooldown)
      Attack();
    else
      ChasePlayer();
  }

  void ChasePlayer()
  {
    anim.SetBool("isFlying", true);

    float dir = Mathf.Sign(player.position.x - transform.position.x);

    Vector2 targetPos = new Vector2(
        player.position.x - dir * attackRange,
        player.position.y
    );

    transform.position = Vector2.MoveTowards(
        transform.position,
        targetPos,
        flySpeed * Time.deltaTime
    );
  }

  void Attack()
  {
    anim.SetBool("isFlying", false);
    anim.SetTrigger("attack");
    lastAttackTime = Time.time;
  }

  void ReturnToStatue()
  {
    if (statuePoint == null) return;

    Face(statuePoint.position.x);

    float dist = Vector2.Distance(transform.position, statuePoint.position);

    if (dist > returnDistance)
    {
      anim.SetBool("isFlying", true);
      transform.position = Vector2.MoveTowards(
          transform.position,
          statuePoint.position,
          flySpeed * Time.deltaTime
      );
    }
    else
    {
      anim.SetBool("isFlying", false);
      transform.position = statuePoint.position;
    }
  }

  void Face(float targetX)
  {
    Vector3 scale = originalScale;

    if (targetX > transform.position.x)
      scale.x = -Mathf.Abs(originalScale.x); // nhìn sang phải
    else
      scale.x = Mathf.Abs(originalScale.x);  // nhìn sang trái

    transform.localScale = scale;
  }


  public void SetAggro(bool value)
  {
    isAggro = value;
  }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
