using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackMode
{
  Normal,
  FireSkill,
  CrescentCross,
  FlameCrescent
}

public class PlayerCombat : MonoBehaviour
{
  [Header("Damage")]
  public int baseDamage = 15;
  public float baseCooldown = 0.3f;

  [Header("Attack Area")]
  public Transform attackPoint;
  public float attackRange = 0.8f;
  public LayerMask enemyLayer;

  [Header("Combo")]
  public float comboResetTime = 0.8f;
  public int maxCombo = 3;

  [Header("Skill Mode")]
  public AttackMode currentAttackMode = AttackMode.Normal;

  Animator animator;
  PlayerStats stats;
  PlayerInputActions input;

  float lastAttackTime;
  float cooldownTimer;
  int attackIndex = 0;

  // =========================
  // UNITY LIFECYCLE
  // =========================
  void Awake()
  {
    animator = GetComponent<Animator>();
    stats = GetComponent<PlayerStats>();
    input = new PlayerInputActions();
  }

  void OnEnable()
  {
    input.Player.Enable();

    // Attack thường (J)
    input.Player.Attack.started += OnNormalAttack;

    // Skill modifier (Q)
    input.Player.SkillModifier.started += _ => TryUseSkill(AttackMode.FireSkill);
  }

  void OnDisable()
  {
    input.Player.Attack.started -= OnNormalAttack;

    input.Player.SkillModifier.started -= _ => TryUseSkill(AttackMode.FireSkill);

    input.Player.Disable();
  }

  void Update()
  {
    cooldownTimer -= Time.deltaTime;

    // reset combo nếu quá thời gian
    if (attackIndex > 0 && Time.time - lastAttackTime > comboResetTime)
    {
      ResetCombo();
    }
  }

  // =========================
  // INPUT CALLBACKS
  // =========================

  // J – đánh thường
  void OnNormalAttack(InputAction.CallbackContext ctx)
  {
    if (cooldownTimer > 0) return;

    currentAttackMode = AttackMode.Normal;
    DoAttack();
  }

  // Dùng chiêu bằng Q
  void TryUseSkill(AttackMode mode)
  {
    if (cooldownTimer > 0) return;

    PlayerHealth health = GetComponent<PlayerHealth>();
    if (health != null && !health.UseMana(20))
    {
      // Không đủ mana
      return;
    }

    currentAttackMode = mode;
    DoAttack();
  }

  // =========================
  // ATTACK CORE
  // =========================
  void DoAttack()
  {
    if (Time.time - lastAttackTime > comboResetTime)
      attackIndex = 0;

    attackIndex++;
    if (attackIndex > maxCombo)
      attackIndex = 1;

    animator.SetInteger("attackIndex", attackIndex);
    animator.SetTrigger("attack");

    lastAttackTime = Time.time;
    cooldownTimer = baseCooldown;
  }

  // =========================
  // ANIMATION EVENTS
  // =========================

  // GỌI Ở CUỐI ATTACK (frame cuối)
  public void ResetCombo()
  {
    attackIndex = 0;
    animator.SetInteger("attackIndex", 0);
    animator.ResetTrigger("attack");
    currentAttackMode = AttackMode.Normal;
  }

  // GỌI Ở FRAME CHÉM
  public void DealDamage(float multiplier)
  {
    Collider2D[] hits = Physics2D.OverlapCircleAll(
        attackPoint.position,
        attackRange,
        enemyLayer
    );

    if (hits.Length == 0) return;

    foreach (Collider2D hit in hits)
    {
      bool crit;
      int dmg = CalculateDamage(out crit);
      dmg = Mathf.RoundToInt(dmg * multiplier);

      // Quái thường
      if (hit.TryGetComponent(out EnemyHealth enemy))
      {
        enemy.TakeDamage(dmg);
      }

      // Boss
      if (hit.TryGetComponent(out BossHealth boss))
      {
        boss.TakeDamage(dmg, crit);
      }
    }
  }

  // =========================
  // DAMAGE CALC
  // =========================
  int CalculateDamage(out bool crit)
  {
    int dmg = baseDamage + stats.bonusDamage;
    crit = Random.value < stats.critRate;
    if (crit) dmg = Mathf.RoundToInt(dmg * 1.5f);
    return dmg;
  }

  // =========================
  // GIZMOS
  // =========================
  void OnDrawGizmosSelected()
  {
    if (!attackPoint) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
  }
}
