using UnityEngine;

public class EarthquakeShockwave : MonoBehaviour
{
    public int damage = 20;
    public float speed = 10f;
    public float lifeTime = 2f;
    
    private bool _hasHit = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        
        // Có thể scale dần ra xa hoặc chỉ là một tia chạy ngang
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                // Chỉ nhận sát thương nếu Player không đang nhảy (hoặc check ground)
                // Đơn giản nhất là luôn trúng nếu dầm phím nhảy không kịp
                hp.TakeDamage(damage);
                _hasHit = true;
            }
        }
    }
}
