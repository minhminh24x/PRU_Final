using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI cho 3 ô Hotbar hiển thị trên HUD.
/// Tạo 3 slot UI, gắn icon + số lượng, highlight slot active.
/// </summary>
public class HotbarUI : MonoBehaviour
{
    [Header("Slot References (kéo 3 ô vào)")]
    public Image[] slotBackgrounds;    // Ô nền (3 cái)
    public Image[] slotIcons;          // Icon item (3 cái)
    public TextMeshProUGUI[] slotQuantityTexts; // Số lượng (3 cái)

    [Header("Colors")]
    public Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color selectedColor = new Color(1f, 0.8f, 0f, 0.9f);

    bool _subscribed = false;

    void LateUpdate()
    {
        // Thử subscribe nếu chưa (giải quyết timing issue)
        if (!_subscribed)
        {
            if (HotbarManager.Instance != null)
            {
                HotbarManager.Instance.OnHotbarChanged += RefreshUI;
                _subscribed = true;
            }
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        }

        // Luôn refresh mỗi frame để đảm bảo UI luôn đúng
        RefreshUI();
    }

    void OnDisable()
    {
        if (HotbarManager.Instance != null)
            HotbarManager.Instance.OnHotbarChanged -= RefreshUI;
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        _subscribed = false;
    }

    public void RefreshUI()
    {
        if (HotbarManager.Instance == null) return;

        for (int i = 0; i < 3; i++)
        {
            ItemData item = HotbarManager.Instance.slotItems[i];

            // Highlight slot đang chọn
            if (slotBackgrounds != null && i < slotBackgrounds.Length && slotBackgrounds[i] != null)
            {
                slotBackgrounds[i].color = (i == HotbarManager.Instance.ActiveSlot)
                    ? selectedColor : normalColor;
            }

            // Icon
            if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
            {
                if (item != null && item.icon != null)
                {
                    slotIcons[i].sprite = item.icon;
                    slotIcons[i].color = Color.white;
                }
                else
                {
                    slotIcons[i].sprite = null;
                    slotIcons[i].color = Color.clear;
                }
            }

            // Số lượng
            if (slotQuantityTexts != null && i < slotQuantityTexts.Length && slotQuantityTexts[i] != null)
            {
                if (item != null && InventoryManager.Instance != null)
                {
                    int count = InventoryManager.Instance.GetItemCount(item);
                    slotQuantityTexts[i].text = count > 0 ? count.ToString() : "";
                }
                else
                {
                    slotQuantityTexts[i].text = "";
                }
            }
        }
    }
}
