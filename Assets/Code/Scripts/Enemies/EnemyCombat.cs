using UnityEngine;

[DisallowMultipleComponent]
public class EnemyCombat : MonoBehaviour
{
    [Header("Attack Stats (Fallback nếu không có EnemyData)")]
    [SerializeField] float fallbackAttackCooldown = 1.0f;
    [SerializeField] int fallbackDamage = 1;

    [Header("Hitbox")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] LayerMask playerLayer;

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

        if (animDriver != null)
            animDriver.PlayAttack();
        else
            Invoke(nameof(DealDamage), 0.5f);

        return true;
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitPlayers =
            Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        int damageToDeal = (_health != null && _health.data != null) ? _health.data.damage : fallbackDamage;

        foreach (Collider2D player in hitPlayers)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(damageToDeal);
        }
    }
}
