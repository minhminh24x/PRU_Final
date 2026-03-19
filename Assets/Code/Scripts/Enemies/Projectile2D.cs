using UnityEngine;

[DisallowMultipleComponent]
public class Projectile2D : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float lifeTime = 5f;

    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D col;

    int _damage = 1;
    float _speed = 8f;
    LayerMask _environmentMask;

    bool _hasHit;

    void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (col == null) col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        _hasHit = false;
    }

    /// <summary>
    /// Combat gọi để cấu hình damage/speed/mask trước khi bắn.
    /// </summary>
    public void Setup(int damage, float speed, LayerMask environmentMask)
    {
        _damage = damage;
        _speed = speed;
        _environmentMask = environmentMask;
    }

    /// <summary>
    /// Bắn theo hướng dir (normalized hoặc không đều được).
    /// </summary>
    public void Fire(Vector2 dir)
    {
        if (rb == null) return;

        Vector2 nd = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;

        // Bay
        rb.linearVelocity = nd * _speed;

        // Xoay mũi tên theo hướng bay (tuỳ bạn có muốn không)
        float angle = Mathf.Atan2(nd.y, nd.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Auto destroy
        CancelInvoke(nameof(SelfDestruct));
        Invoke(nameof(SelfDestruct), lifeTime);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_hasHit) return; // chống double hit
        _hasHit = true;

        // 1) Hit Player => damage
        PlayerHealth hp = collision.collider.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.TakeDamage(_damage);
            Destroy(gameObject);
            return;
        }

        // 2) Hit Environment => destroy (tường/ground)
        if (((1 << collision.gameObject.layer) & _environmentMask) != 0)
        {
            Destroy(gameObject);
            return;
        }

        // 3) Hit thứ khác => cũng destroy cho an toàn
        Destroy(gameObject);
    }
}
