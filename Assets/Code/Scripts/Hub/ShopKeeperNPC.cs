using UnityEngine;
using TMPro;

/// <summary>
/// Gắn lên GameObject NPC Shopkeeper trong Hub Map.
/// Khi Player đến gần + nhấn E → mở UI cửa hàng.
/// </summary>
public class ShopKeeperNPC : MonoBehaviour
{
    [Header("Dữ liệu Shop")]
    public ShopData shopData;

    [Header("UI Prompt")]
    [Tooltip("TextMeshPro 3D phía trên NPC: '[E] Mua bán'")]
    public TextMeshPro interactPrompt;

    private bool playerNearby = false;
    private bool isShopOpen = false;

    void Start()
    {
        if (interactPrompt) interactPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!playerNearby || isShopOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
            OpenShop();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (interactPrompt) interactPrompt.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (interactPrompt) interactPrompt.gameObject.SetActive(false);

        // Tự đóng shop nếu Player đi ra
        if (isShopOpen) ForceCloseShop();
    }

    void OpenShop()
    {
        if (ShopUIManager.Instance == null)
        {
            Debug.LogWarning("[ShopKeeperNPC] Không tìm thấy ShopUIManager trong scene!");
            return;
        }

        isShopOpen = true;
        if (interactPrompt) interactPrompt.gameObject.SetActive(false);

        ShopUIManager.Instance.OpenShop(shopData);
        // Đăng ký callback khi player đóng shop bằng E/Esc (dùng polling đơn giản)
        StartCoroutine(WaitForShopClose());
    }

    void ForceCloseShop()
    {
        isShopOpen = false;
        if (ShopUIManager.Instance != null)
            ShopUIManager.Instance.CloseShop();
    }

    System.Collections.IEnumerator WaitForShopClose()
    {
        // Chờ đến khi shopPanel đóng lại
        yield return new WaitUntil(() =>
            ShopUIManager.Instance == null || !ShopUIManager.Instance.shopPanel.activeSelf);

        isShopOpen = false;
        if (playerNearby && interactPrompt)
            interactPrompt.gameObject.SetActive(true);
    }
}
