using UnityEngine;

[DisallowMultipleComponent]
public class EnemyRangedCombat : MonoBehaviour
{
    [Header("Attack Stats")]
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int damage = 1;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPoint;
    [SerializeField] float projectileSpeed = 8f;

    [Header("Hit Filters")]
    [Tooltip("Layer của tường/ground để projectile xử lý va chạm (tuỳ Projectile2D dùng).")]
    [SerializeField] LayerMask environmentMask;

    [Header("References")]
    [SerializeField] EnemyAnimDriver animDriver;
    [Tooltip("Nếu không có animDriver/anim event, sẽ bắn sau delay này.")]
    [SerializeField] float fallbackShootDelay = 0.2f;

    float _nextAttackTime;

    // Lưu target cho animation event dùng
    Transform _pendingTarget;

    void Reset()
    {
        animDriver = GetComponent<EnemyAnimDriver>();
    }

    void Awake()
    {
        if (animDriver == null)
            animDriver = GetComponent<EnemyAnimDriver>();
    }

    /// <summary>
    /// Controller gọi hàm này. Hàm sẽ trigger attack anim và lưu target.
    /// Animation Event trong clip Attack sẽ gọi Shoot().
    /// </summary>
    public bool TryAttack(Transform target)
    {
        if (Time.time < _nextAttackTime) return false;
        if (target == null) return false;
        if (projectilePrefab == null || shootPoint == null) return false;

        _nextAttackTime = Time.time + attackCooldown;
        _pendingTarget = target;

        if (animDriver != null)
        {
            animDriver.PlayAttack();
        }
        else
        {
            // Fallback: không có animator thì bắn thẳng sau delay
            Invoke(nameof(Shoot), fallbackShootDelay);
        }

        return true;
    }

    /// <summary>
    /// Gọi từ Animation Event (khuyên dùng), hoặc fallback Invoke.
    /// </summary>
    public void Shoot()
    {
        Debug.Log($"[RangedCombat] Shoot() pendingTarget={(_pendingTarget ? _pendingTarget.name : "NULL")}", this);

        if (projectilePrefab == null || shootPoint == null) return;

        if (_pendingTarget == null || !_pendingTarget.gameObject.activeInHierarchy)
        {
            Debug.Log("[RangedCombat] Shoot() aborted: target invalid", this);
            return;
        }
        if (projectilePrefab == null || shootPoint == null) return;

        // Nếu target mất (die/disable) thì không bắn
        if (_pendingTarget == null || !_pendingTarget.gameObject.activeInHierarchy)
            return;

        Vector2 from = shootPoint.position;
        Vector2 to = _pendingTarget.position;
        Vector2 dir = (to - from).normalized;

        GameObject go = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        // Nếu projectile có script Projectile2D thì feed dữ liệu vào
        Projectile2D proj = go.GetComponent<Projectile2D>();
        if (proj != null)
        {
            proj.Setup(damage, projectileSpeed, environmentMask);
            proj.Fire(dir);
        }
        else
        {
            // Nếu chưa có Projectile2D, cố gắng set velocity trực tiếp để bạn thấy nó bay
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = dir * projectileSpeed;
        }
    }
}
