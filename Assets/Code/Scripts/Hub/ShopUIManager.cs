using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Singleton quản lý UI cửa hàng trong Hub Map.
/// Hiển thị danh sách hàng, gold hiện tại, và xử lý mua hàng.
/// </summary>
public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance { get; private set; }

    [Header("UI References – Panel chính")]
    public GameObject shopPanel;
    public TextMeshProUGUI shopkeeperNameText;
    public Image shopkeeperPortrait;
    public TextMeshProUGUI greetingText;
    public TextMeshProUGUI playerGoldText;  // Hiển thị "Gold: 150"

    [Header("Item List")]
    public Transform itemListParent;        // Content bên trong ScrollView
    public GameObject shopItemRowPrefab;    // Prefab 1 hàng item (xem mô tả bên dưới)

    [Header("Thông báo")]
    public TextMeshProUGUI feedbackText;    // "Mua thành công!", "Không đủ Gold!", v.v.
    public float feedbackDuration = 2f;

    // --- Trạng thái ---
    private ShopData currentShop;
    private List<int> stockRemaining = new List<int>();
    private List<GameObject> spawnedRows = new List<GameObject>();
    private Coroutine feedbackCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        shopPanel.SetActive(false);

        // Lắng nghe thay đổi Gold để cập nhật UI realtime
        CurrencyManager.OnGoldChanged += RefreshGoldText;
    }

    void OnDestroy()
    {
        CurrencyManager.OnGoldChanged -= RefreshGoldText;
    }

    void Update()
    {
        // Nhấn Escape hoặc E để đóng shop
        if (shopPanel.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
        {
            CloseShop();
        }
    }

    // ─── PUBLIC API ────────────────────────────────────────────────────────────

    public void OpenShop(ShopData shopData)
    {
        if (shopData == null) return;
        currentShop = shopData;

        // Setup header
        if (shopkeeperNameText) shopkeeperNameText.text = shopData.shopkeeperName;
        if (greetingText) greetingText.text = shopData.greetingLine;
        if (shopkeeperPortrait)
        {
            shopkeeperPortrait.gameObject.SetActive(shopData.shopkeeperPortrait != null);
            if (shopData.shopkeeperPortrait) shopkeeperPortrait.sprite = shopData.shopkeeperPortrait;
        }

        RefreshGoldText(CurrencyManager.Gold);

        // Build danh sách stock
        stockRemaining.Clear();
        foreach (var entry in shopData.items)
            stockRemaining.Add(entry.stockLimit); // -1 = unlimited

        BuildItemList();

        shopPanel.SetActive(true);
        LockPlayer(true);
        if (feedbackText) feedbackText.text = "";
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        LockPlayer(false);
        currentShop = null;
    }

    // ─── NÚT MUA – Gọi từ mỗi Row ─────────────────────────────────────────────

    /// <param name="entryIndex">Vị trí item trong ShopData.items</param>
    public void BuyItem(int entryIndex)
    {
        if (currentShop == null) return;
        if (entryIndex < 0 || entryIndex >= currentShop.items.Length) return;

        ShopEntry entry = currentShop.items[entryIndex];
        int price = entry.GetPrice();

        // Kiểm tra hết hàng
        if (stockRemaining[entryIndex] == 0)
        {
            ShowFeedback("<color=red>Hết hàng!</color>");
            return;
        }

        // Kiểm tra gold
        if (!CurrencyManager.HasEnough(price))
        {
            ShowFeedback($"<color=red>Không đủ Gold! (cần {price}G)</color>");
            return;
        }

        // Kiểm tra kho còn chỗ
        if (InventoryManager.Instance != null && !InventoryManager.Instance.AddItem(entry.item, 1))
        {
            ShowFeedback("<color=red>Kho đồ đầy!</color>");
            return;
        }

        // Trừ gold
        CurrencyManager.SpendGold(price);

        // Giảm stock
        if (stockRemaining[entryIndex] > 0)
        {
            stockRemaining[entryIndex]--;
        }

        ShowFeedback($"<color=green>Đã mua {entry.item.itemName}! (-{price}G)</color>");
        RefreshItemRow(entryIndex);
    }

    // ─── PRIVATE HELPERS ───────────────────────────────────────────────────────

    void BuildItemList()
    {
        // Xóa các row cũ
        foreach (var go in spawnedRows) Destroy(go);
        spawnedRows.Clear();

        if (currentShop.items == null) return;

        for (int i = 0; i < currentShop.items.Length; i++)
        {
            var entry = currentShop.items[i];
            if (entry.item == null) continue;

            GameObject row = Instantiate(shopItemRowPrefab, itemListParent);
            spawnedRows.Add(row);
            SetupItemRow(row, entry, i, stockRemaining[i]);
        }
    }

    /// <summary>
    /// Điền thông tin vào 1 hàng. Row prefab cần có các TextMeshPro + Button đặt tên đúng.
    /// </summary>
    void SetupItemRow(GameObject row, ShopEntry entry, int index, int stock)
    {
        // Icon
        var icon = row.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (icon && entry.item.icon) icon.sprite = entry.item.icon;

        // Tên item
        var nameText = row.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        if (nameText) nameText.text = entry.item.itemName;

        // Mô tả ngắn
        var descText = row.transform.Find("ItemDesc")?.GetComponent<TextMeshProUGUI>();
        if (descText) descText.text = entry.item.description;

        // Giá
        var priceText = row.transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();
        if (priceText) priceText.text = $"{entry.GetPrice()} G";

        // Stock
        var stockText = row.transform.Find("StockText")?.GetComponent<TextMeshProUGUI>();
        if (stockText) stockText.text = stock < 0 ? "∞" : $"Còn: {stock}";

        // Button mua
        var buyBtn = row.transform.Find("BuyButton")?.GetComponent<Button>();
        if (buyBtn)
        {
            int capturedIndex = index; // capture for lambda
            buyBtn.onClick.RemoveAllListeners();
            buyBtn.onClick.AddListener(() => BuyItem(capturedIndex));
            buyBtn.interactable = stock != 0;
        }
    }

    void RefreshItemRow(int index)
    {
        if (index >= spawnedRows.Count) return;
        var entry = currentShop.items[index];
        SetupItemRow(spawnedRows[index], entry, index, stockRemaining[index]);
    }

    void RefreshGoldText(int gold)
    {
        if (playerGoldText) playerGoldText.text = $"Gold: {gold} G";
    }

    void ShowFeedback(string msg)
    {
        if (!feedbackText) return;
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(FeedbackRoutine(msg));
    }

    System.Collections.IEnumerator FeedbackRoutine(string msg)
    {
        feedbackText.text = msg;
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.text = "";
    }

    void LockPlayer(bool locked)
    {
        var movement = FindFirstObjectByType<PlayerMovement>();
        if (movement) movement.enabled = !locked;
    }
}
