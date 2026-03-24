using UnityEngine;
using TMPro;

/// <summary>
/// Gắn lên prefab viên ngọc rơi ra khi boss chết.
/// Player đến gần + nhấn E → thu thập ngọc và báo GemManager.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GemPickup : MonoBehaviour
{
    [Header("Dữ liệu ngọc")]
    public GemData gemData;

    [Header("UI Prompt")]
    [Tooltip("TextMeshPro 3D nhỏ phía trên ngọc, ví dụ: '[E] Nhặt Ngọc'")]
    public TextMeshPro interactPrompt;

    [Header("Hiệu ứng")]
    public float bobSpeed    = 1.5f;
    public float bobHeight   = 0.15f;

    private bool playerNearby;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);

        // Đảm bảo Collider2D là trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        // Bob up-down animation
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (!playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
            Collect();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;

        if (interactPrompt != null)
            interactPrompt.gameObject.SetActive(false);
    }

    void Collect()
    {
        if (gemData == null)
        {
            Debug.LogWarning("[GemPickup] Chưa gán GemData!");
            return;
        }

        if (GemManager.Instance == null)
        {
            Debug.LogWarning("[GemPickup] Không tìm thấy GemManager trong scene!");
            return;
        }

        GemManager.Instance.CollectGem(gemData);
        Destroy(gameObject);
    }
}
