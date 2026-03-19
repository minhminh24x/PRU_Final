using UnityEngine;

public class BossAttack : MonoBehaviour
{
  public int damage = 35;          // 👈 SÁT THƯƠNG QUÁI
  public Transform attackPoint;
  public float attackRange = 0.8f;
  public LayerMask playerLayer;

  public float attackCooldown = 1.2f;
  float timer;

  void Update()
  {
    timer -= Time.deltaTime;
  }

  public void TryAttack()
  {
    if (timer > 0) return;

    Collider2D hit = Physics2D.OverlapCircle(
        attackPoint.position,
        attackRange,
        playerLayer
    );

    if (hit != null && hit.TryGetComponent(out PlayerHealth player))
    {
      player.TakeDamage(damage);
      timer = attackCooldown;
    }
  }

  void OnDrawGizmosSelected()
  {
    if (attackPoint == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
  }
}
