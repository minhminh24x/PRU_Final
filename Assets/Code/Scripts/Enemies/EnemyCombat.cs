using UnityEngine;

[DisallowMultipleComponent]
public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Stats (Fallback nếu không có EnemyData)")]
    [SerializeField] float fallbackAttackCooldown = 1.0f;
    [SerializeField] int fallbackDamage = 1;

    [Header("Hitbox")]
    [Tooltip("Điểm xuất phát sát thương (Để trống thì tự lấy tâm con quái)")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 1.0f;
    [Tooltip("Layer của Player (Để Nothing nó sẽ tự động tìm layer tên 'Player')")]
    [SerializeField] LayerMask playerLayer;

    [Header("Damage Timing")]
    [Tooltip("Thời gian kể từ lúc giơ tay đến lúc tính sát thương (0.5s). Nếu bạn xài AnimationEvent thì set cái này về 0.")]
    [SerializeField] float damageDelay = 0.5f;

    [Header("References")]
    [SerializeField] EnemyAnimDriver animDriver;
    EnemyHealth _health;

    float _nextAttackTime;

    void Awake()
    {
        if (animDriver == null)
            animDriver = GetComponent<EnemyAnimDriver>();
        
        _health = GetComponent<EnemyHealth>();
    }

    public bool TryAttack()
    {
        if (Time.time < _nextAttackTime) return false;

        float cooldown = (_health != null && _health.data != null) ? _health.data.attackCooldown : fallbackAttackCooldown;
        _nextAttackTime = Time.time + cooldown;

        // Bật hoạt ảnh vung vũ khí
        if (animDriver != null)
            animDriver.PlayAttack();

        // Tự động tính sát thương luôn sau 0.5s (dành cho newbie chưa biết cắm Animation Event)
        if (damageDelay > 0f)
            Invoke(nameof(DealDamage), damageDelay);

        return true;
    }

    public void DealDamage()
    {
        // 1. Nếu quên gắn AttackPoint thì tự lấy thân con cá làm tâm vòng sát thương
        Transform point = attackPoint != null ? attackPoint : transform;

        // 2. Nếu quên chọn LayerMask thì tự động đi tìm Layer tên là "Player"
        if (playerLayer.value == 0)
        {
            playerLayer = LayerMask.GetMask("Player");
            if (playerLayer.value == 0)
            {
                Debug.LogWarning("[EnemyCombat] Game của bạn chưa tạo Layer nào tên là 'Player' trong danh sách Layer! Hãy đi tạo và dán cho nhân vật.");
            }
        }

        Collider2D[] hitPlayers =
            Physics2D.OverlapCircleAll(point.position, attackRadius, playerLayer);

        int damageToDeal = (_health != null && _health.data != null) ? _health.data.damage : fallbackDamage;

        // Lưu danh sách những Player đã bị đánh trúng để không trừ máu 2 lần nếu quẹt trúng 2 collider
        System.Collections.Generic.HashSet<PlayerHealth> damagedPlayers = new System.Collections.Generic.HashSet<PlayerHealth>();

        foreach (Collider2D playerHit in hitPlayers)
        {
            // Tìm PlayerHealth trên chính nó hoặc trên Object Cha chứa nó
            PlayerHealth hp = playerHit.GetComponentInParent<PlayerHealth>();
            if (hp != null && !damagedPlayers.Contains(hp))
            {
                hp.TakeDamage(damageToDeal);
                damagedPlayers.Add(hp);
                Debug.Log($"[EnemyCombat] Đã cắn trúng thằng {hp.name}, mất {damageToDeal} máu!");
            }
        }
    }
}
