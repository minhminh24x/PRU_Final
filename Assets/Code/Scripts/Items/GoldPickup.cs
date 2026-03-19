using UnityEngine;

/// <summary>
/// Vật phẩm vàng rơi ra từ quái.
/// Player chạm vào = tự động nhặt (không cần bấm E).
/// </summary>
public class GoldPickup : MonoBehaviour
{
    [Header("Số vàng")]
    public int amount = 1;

    [Header("Hiệu ứng nhặt")]
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float floatAmplitude = 0.15f;

    Vector3 _startPos;

    void Start()
    {
        SnapToGround();
        _startPos = transform.position;
    }

    void SnapToGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 15f);
        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger) continue; // Bỏ qua trigger
            if (hit.collider.gameObject == gameObject) continue; // Bỏ qua bản thân
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy")) continue;

            // Đặt vàng ngay tại mặt đất (offset 0.2f để không lún)
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.2f, transform.position.z);
            break;
        }
    }

    void Update()
    {
        // Hiệu ứng lơ lửng nhẹ
        float newY = _startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(_startPos.x, newY, _startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CurrencyManager.AddGold(amount);
        Destroy(gameObject);
    }
}
