using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Prefab script cho mỗi ô item trong grid Inventory.
/// Hiển thị icon + số lượng. Click để gắn vào hotbar hoặc mặc equipment.
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public Button button;

    InventorySlot _slot;
    InventoryUI _parentUI;

    public void Setup(InventorySlot slot, InventoryUI parentUI)
    {
        _slot = slot;
        _parentUI = parentUI;

        if (slot != null && slot.item != null)
        {
            iconImage.sprite = slot.item.icon;
            iconImage.color = Color.white;
            quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1, 1, 1, 0.1f);
            quantityText.text = "";
            button.onClick.RemoveAllListeners();
        }
    }

    void OnClick()
    {
        if (_slot == null || _slot.item == null) return;

        // Equipment → mặc đồ
        if (_slot.item.itemType == ItemType.Equipment)
        {
            if (EquipmentManager.Instance != null)
                EquipmentManager.Instance.Equip(_slot.item);

            if (_parentUI != null) _parentUI.RefreshUI();
            return;
        }

        // Consumable → gắn vào slot hotbar đang chọn
        if (_slot.item.itemType == ItemType.Consumable)
        {
            if (HotbarManager.Instance != null)
            {
                int activeSlot = HotbarManager.Instance.ActiveSlot;
                HotbarManager.Instance.AssignItem(activeSlot, _slot.item);
                Debug.Log($"<color=yellow>Gắn {_slot.item.itemName} vào Hotbar slot {activeSlot + 1}</color>");
            }
        }
    }
}
