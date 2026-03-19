using UnityEngine;

/// <summary>
/// Vật phẩm rơi trên mặt đất.
/// Bấm E để nhặt vào Inventory (không dùng trực tiếp nữa).
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("Item Data (kéo ScriptableObject vào)")]
    public ItemData itemData;
    public int quantity = 1;

    [Header("UX/UI")]
    public GameObject hintUI;

    bool isPlayerNearby = false;

    void Start()
    {
        if (hintUI != null) hintUI.SetActive(false);
        SnapToGround();
    }

    void SnapToGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 15f);
        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger) continue;
            if (hit.collider.gameObject == gameObject) continue;
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy")) continue;

            transform.position = new Vector3(transform.position.x, hit.point.y + 0.3f, transform.position.z);
            break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = true;
        if (hintUI != null) hintUI.SetActive(true);

        // Báo cho HotbarManager biết có item gần
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.SetNearbyPickup(this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        if (hintUI != null) hintUI.SetActive(false);

        if (HotbarManager.Instance != null)
            HotbarManager.Instance.ClearNearbyPickup(this);
    }

    /// <summary>
    /// Nhặt item vào Inventory (được gọi bởi HotbarManager khi E).
    /// </summary>
    public void PickUpToInventory()
    {
        if (itemData == null) return;

        if (InventoryManager.Instance != null)
        {
            bool added = InventoryManager.Instance.AddItem(itemData, quantity);
            if (!added)
            {
                Debug.Log("<color=red>Kho đồ đầy, không nhặt được!</color>");
                return;
            }

            // Tự động gán vào hotbar nếu là consumable
            if (HotbarManager.Instance != null)
                HotbarManager.Instance.AutoAssign(itemData);
        }

        Debug.Log($"<color=green>Nhặt {quantity}x {itemData.itemName}</color>");

        // Cleanup
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.ClearNearbyPickup(this);

        Destroy(gameObject);
    }
}