using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Panel Inventory — bấm I để mở/đóng.
/// Bên trái: nhân vật + 4 slot trang bị.
/// Giữa: grid kho đồ.
/// Trên: tabs (Kho đồ, Chiêu thức).
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Panel Root (ẩn/hiện khi bấm I)")]
    public GameObject inventoryPanel;

    [Header("Tabs")]
    public Button tabInventory;
    public Button tabSkills;
    public GameObject inventoryContent;
    public GameObject skillsContent;

    [Header("Item Grid")]
    public Transform gridParent;          // Content của ScrollView
    public GameObject slotPrefab;         // Prefab InventorySlotUI

    [Header("Equipment Slots (kéo 4 Image vào)")]
    public Image helmetSlotIcon;
    public Image armorSlotIcon;
    public Image bootsSlotIcon;
    public Image accessorySlotIcon;

    [Header("Equipment Buttons (kéo 4 Button vào)")]
    public Button helmetButton;
    public Button armorButton;
    public Button bootsButton;
    public Button accessoryButton;

    [Header("Info Panel")]
    public TextMeshProUGUI itemInfoText;

    bool isOpen = false;

    void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        // Tab buttons
        if (tabInventory != null) tabInventory.onClick.AddListener(() => SwitchTab(0));
        if (tabSkills != null) tabSkills.onClick.AddListener(() => SwitchTab(1));

        // Equipment unequip buttons
        if (helmetButton != null) helmetButton.onClick.AddListener(() => UnequipSlot(EquipSlot.Helmet));
        if (armorButton != null) armorButton.onClick.AddListener(() => UnequipSlot(EquipSlot.Armor));
        if (bootsButton != null) bootsButton.onClick.AddListener(() => UnequipSlot(EquipSlot.Boots));
        if (accessoryButton != null) accessoryButton.onClick.AddListener(() => UnequipSlot(EquipSlot.Accessory));
    }

    void OnEnable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.OnEquipmentChanged += RefreshUI;
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.OnEquipmentChanged -= RefreshUI;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        if (inventoryPanel != null) inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
            // Pause game (optional)
            // Time.timeScale = 0f;
        }
        else
        {
            // Time.timeScale = 1f;
        }
    }

    public void RefreshUI()
    {
        RefreshGrid();
        RefreshEquipment();
    }

    void RefreshGrid()
    {
        if (gridParent == null || slotPrefab == null) return;
        if (InventoryManager.Instance == null) return;

        // Xóa slot cũ an toàn (dùng vòng lặp ngược để tránh lỗi skip)
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Transform child = gridParent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        // Tạo slot mới cho mỗi item trong inventory
        foreach (var slot in InventoryManager.Instance.slots)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
                slotUI.Setup(slot, this);
        }

        // Thêm slot trống cho đủ min 20 ô
        int emptySlots = InventoryManager.Instance.maxSlots - InventoryManager.Instance.slots.Count;
        for (int i = 0; i < emptySlots; i++)
        {
            GameObject obj = Instantiate(slotPrefab, gridParent);
            InventorySlotUI slotUI = obj.GetComponent<InventorySlotUI>();
            if (slotUI != null)
                slotUI.Setup(null, this);
        }
    }

    void RefreshEquipment()
    {
        if (EquipmentManager.Instance == null) return;

        SetEquipIcon(helmetSlotIcon, EquipmentManager.Instance.helmet);
        SetEquipIcon(armorSlotIcon, EquipmentManager.Instance.armor);
        SetEquipIcon(bootsSlotIcon, EquipmentManager.Instance.boots);
        SetEquipIcon(accessorySlotIcon, EquipmentManager.Instance.accessory);
    }

    void SetEquipIcon(Image img, ItemData item)
    {
        if (img == null) return;
        if (item != null && item.icon != null)
        {
            img.sprite = item.icon;
            img.color = Color.white;
        }
        else
        {
            img.sprite = null;
            img.color = new Color(1, 1, 1, 0.15f);
        }
    }

    void UnequipSlot(EquipSlot slot)
    {
        if (EquipmentManager.Instance != null)
            EquipmentManager.Instance.Unequip(slot);
    }

    void SwitchTab(int tab)
    {
        if (inventoryContent != null) inventoryContent.SetActive(tab == 0);
        if (skillsContent != null) skillsContent.SetActive(tab == 1);
    }

    public void ShowItemInfo(ItemData item)
    {
        if (itemInfoText == null || item == null) return;

        string info = $"<color=yellow><size=150%><b>{item.itemName}</b></size></color>\n";
        info += $"<color=white>{item.description}</color>\n\n";

        // Thêm thông số phụ nếu có
        if (item.itemType == ItemType.Consumable)
        {
            if (item.healAmount > 0) info += $"<color=green>Hồi phục: {item.healAmount} HP</color>\n";
            if (item.manaAmount > 0) info += $"<color=#00BFFF>Hồi phục: {item.manaAmount} MP</color>\n";
            if (item.buffDamagePercent > 0) info += $"<color=red>Buff Sát thương: +{item.buffDamagePercent}%</color> ({item.buffDuration}s)\n";
        }
        else if (item.itemType == ItemType.Equipment)
        {
            if (item.bonusDEF > 0) info += $"<color=orange>Thủ: +{item.bonusDEF}</color>\n";
            if (item.bonusSTR > 0) info += $"<color=red>Sức mạnh: +{item.bonusSTR}</color>\n";
            if (item.bonusINT > 0) info += $"<color=#00BFFF>Trí lực: +{item.bonusINT}</color>\n";
            if (item.bonusAGI > 0) info += $"<color=green>Nhanh nhẹn: +{item.bonusAGI}</color>\n";
        }

        info += $"<color=#AAAAAA><i>Nhấn chuột để dùng / ghép vào hotbar</i></color>";

        itemInfoText.text = info;
    }

    public void HideItemInfo()
    {
        if (itemInfoText != null)
        {
            itemInfoText.text = "";
        }
    }
}
