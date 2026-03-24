using UnityEngine;

public class BossHealth : MonoBehaviour
{
  public int maxHP = 120;
  int currentHP;

  [Header("UI")]
  public HealthBarUI healthBar;
  public DamagePopupSpawner popupSpawner;

  [Header("Gate")]
  public GameObject rightGate;   // 🔒 collider chặn mép phải

  [Header("Gem Drop")]
  [Tooltip("ScriptableObject GemData của ngọc boss này sẽ rơi ra")]
  public GemData gemToDrop;
  [Tooltip("Prefab GemPickup sẽ được spawn tại vị trí boss khi chết")]
  public GameObject gemPrefab;

  bool isDead;
  Animator anim;

  void Start()
  {
    currentHP = maxHP;
    anim = GetComponent<Animator>();

    if (healthBar != null)
      healthBar.Init(maxHP);

    // 🔒 Boss còn sống → KHÓA MÉP
    if (rightGate != null)
      rightGate.SetActive(true);
  }

  public void TakeDamage(int dmg, bool crit)
  {
    if (isDead) return;

    currentHP -= dmg;

    if (healthBar != null)
      healthBar.SetHealth(currentHP);

    if (popupSpawner != null)
      popupSpawner.ShowDamage(dmg, crit);

    if (currentHP <= 0)
      Die();
    else
      Hurt();
  }

  void Hurt()
  {
    if (anim == null) return;

    anim.SetBool("isHurt", true);
    Invoke(nameof(ResetHurt), 0.2f);
  }

  void ResetHurt()
  {
    if (anim == null) return;
    anim.SetBool("isHurt", false);
  }

  void Die()
  {
    isDead = true;

    // 🔓 Boss chết → MỞ MÉP
    if (rightGate != null)
      rightGate.SetActive(false);

    if (anim != null)
      anim.SetBool("isDead", true);

    BossAI ai = GetComponent<BossAI>();
    if (ai != null) ai.enabled = false;

    BossAttack attack = GetComponent<BossAttack>();
    if (attack != null) attack.enabled = false;

    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = false;

    // 💎 Drop ngọc nếu chưa thu thập
    SpawnGemDrop();

    Destroy(gameObject, 2f);
  }

  void SpawnGemDrop()
  {
    if (gemToDrop == null || gemPrefab == null) return;

    // Không spawn nếu đã thu thập từ trước
    if (GemManager.Instance != null && GemManager.Instance.IsCollected(gemToDrop.gemType))
    {
      Debug.Log($"[BossHealth] Ngọc {gemToDrop.gemName} đã thu thập trước đó — không drop lại.");
      return;
    }

    Vector3 dropPos = transform.position + Vector3.up * 1.5f;
    Instantiate(gemPrefab, dropPos, Quaternion.identity);
    Debug.Log($"[BossHealth] Dropped gem: {gemToDrop.gemName}");
  }
}
