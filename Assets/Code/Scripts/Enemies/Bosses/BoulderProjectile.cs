using UnityEngine;

public class BoulderProjectile : MonoBehaviour
{
    public int damage = 15;
    public float rollSpeed = 8f;
    public float rotationSpeed = 360f;
    public float lifeTime = 4f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // Giữ vận tốc lăn ngang không đổi
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
            // Có thể force speed nếu cần: 
            // rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * rollSpeed, rb.linearVelocity.y);
        }

        // Xoay hình ảnh để tạo cảm giác lăn
        transform.Rotate(0, 0, -Mathf.Sign(rb.linearVelocity.x) * rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth hp = collision.collider.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        // Nếu đâm vào tường (Environment), tự vỡ
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
             // Có thể thêm hiệu ứng vỡ ở đây
             Destroy(gameObject);
        }
    }
}
