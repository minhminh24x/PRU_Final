using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Prefab script cho mỗi ô item trong grid Inventory.
/// Hiển thị icon + số lượng. Click để gắn vào hotbar hoặc mặc equipment.
/// Bao gồm tooltip khi hover vào.
/// </summary>
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public Button button;

    InventorySlot _slot;
    InventoryUI _parentUI;

    public void Setup(InventorySlot slot, InventoryUI parentUI)
    {
        // Tự động tìm lại đúng các UI Components mỗi khi Setup
        // (không dùng Awake vì Instantiate dưới parent Inactive sẽ làm Awake trễ)
        Transform iconT = transform.Find("ItemIcon");
        if (iconT != null) iconImage = iconT.GetComponent<Image>();

        Transform amountT = transform.Find("AmountText");
        if (amountT != null) quantityText = amountT.GetComponent<TextMeshProUGUI>();

        if (button == null) button = GetComponent<Button>();

        _slot = slot;
        _parentUI = parentUI;

        if (iconImage != null)
        {
            iconImage.gameObject.SetActive(true);
            iconImage.enabled = true;
            iconImage.preserveAspect = true;
            
            // Ép scale và alpha phòng trường hợp bị ẩn
            iconImage.transform.localScale = Vector3.one;
            CanvasGroup cg = iconImage.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;

            // Chống việc Button làm đổi màu ItemIcon (faint white) nếu lỡ gán làm Target Graphic
            if (button != null && button.targetGraphic == iconImage)
            {
                button.targetGraphic = GetComponent<Image>();
            }

            // Xếp lên trên cùng để không bị background che
            iconImage.transform.SetAsLastSibling();

            // Nếu user setup RectTransform bằng 0, ép nó lấp đầy ô
            RectTransform rt = iconImage.rectTransform;
            if (rt.rect.width <= 1f || rt.rect.height <= 1f)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = new Vector2(5, 5);
                rt.offsetMax = new Vector2(-5, -5);
            }
        }

        // Ưu tiên hiển thị chữ lêm trên hình ảnh vật phẩm
        if (quantityText != null)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.transform.localScale = Vector3.one;
            quantityText.color = Color.white;
            quantityText.alignment = TextAlignmentOptions.BottomRight;
            quantityText.transform.SetAsLastSibling();
        }

        if (slot != null && slot.item != null)
        {
            // Debug.Log($"<color=cyan>Setup UI Slot: {slot.item.itemName} x{slot.quantity}</color>");
            iconImage.sprite = slot.item.icon;
            iconImage.color = Color.white;
            quantityText.text = slot.quantity > 0 ? slot.quantity.ToString() : "";

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slot != null && _slot.item != null && _parentUI != null)
        {
            _parentUI.ShowItemInfo(_slot.item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_parentUI != null)
        {
            _parentUI.HideItemInfo();
        }
    }
}
