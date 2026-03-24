using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Quản lý 3 ô công cụ (Hotbar).
/// Phím 1/2/3 chọn slot. Phím E dùng item tại slot đang chọn.
/// </summary>
public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }

    /// <summary>Set true khi player đứng trong vùng Portal, để Portal có quyền ưu tiên phím E</summary>
    public static bool IsEKeyClaimedByPortal = false;

    [Header("Hotbar Config")]
    public int slotCount = 3;

    /// <summary>Mỗi slot trỏ tới 1 ItemData (hoặc null nếu trống)</summary>
    public ItemData[] slotItems;

    /// <summary>Slot đang được chọn (0, 1, 2)</summary>
    public int ActiveSlot { get; private set; } = 0;

    public event Action OnHotbarChanged;

    // Dùng để check xem Player có đang đứng gần item dưới đất không
    ItemPickup _nearbyPickup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        slotItems = new ItemData[slotCount];
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Phím 1/2/3 chọn slot
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);

        // Phím E: nhường cho Portal hoặc NPC nếu player đang đứng trong vùng tương tác
        if (Keyboard.current.eKey.wasPressedThisFrame && !IsEKeyClaimedByPortal)
        {
            // Nếu đang đứng cạnh Shop hoặc Shop đang mở -> Không dùng item hotbar
            if (ShopKeeperNPC.IsAnyShopNearby || (ShopUIManager.Instance != null && ShopUIManager.Instance.shopPanel.activeSelf))
                return;

            if (_nearbyPickup != null)
            {
                // Ưu tiên nhặt item dưới đất
                _nearbyPickup.PickUpToInventory();
            }
            else
            {
                // Dùng item trong hotbar
                UseActiveSlot();
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slotCount) return;
        ActiveSlot = index;
        OnHotbarChanged?.Invoke();

        string itemName = slotItems[index] != null ? slotItems[index].itemName : "Trống";
        Debug.Log($"<color=yellow>Chọn slot {index + 1}: {itemName}</color>");
    }

    /// <summary>
    /// Gán item vào slot hotbar.
    /// </summary>
    public void AssignItem(int slotIndex, ItemData item)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return;

        // Nếu item này đã nằm ở slot khác, xóa nó khỏi slot cũ
        if (item != null)
        {
            for (int i = 0; i < slotCount; i++)
            {
                if (i != slotIndex && slotItems[i] == item)
                {
                    slotItems[i] = null;
                }
            }
        }

        slotItems[slotIndex] = item;
        OnHotbarChanged?.Invoke();
    }

    /// <summary>
    /// Xóa item khỏi slot hotbar (khi hết quantity trong inventory).
    /// </summary>
    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotCount) return;
        slotItems[slotIndex] = null;
        OnHotbarChanged?.Invoke();
    }

    /// <summary>
    /// Tự động gán consumable vào hotbar khi nhặt.
    /// Ưu tiên: slot có sẵn cùng item > slot trống đầu tiên.
    /// </summary>
    public void AutoAssign(ItemData item)
    {
        if (item == null || (item.itemType != ItemType.Consumable && item.itemType != ItemType.Potion)) return;

        // Ưu tiên 1: slot đã có cùng item
        for (int i = 0; i < slotCount; i++)
        {
            if (slotItems[i] == item)
            {
                OnHotbarChanged?.Invoke();
                return; // Đã có trong hotbar, chỉ cần refresh UI
            }
        }

        // Ưu tiên 2: slot trống đầu tiên
        for (int i = 0; i < slotCount; i++)
        {
            if (slotItems[i] == null)
            {
                slotItems[i] = item;
                OnHotbarChanged?.Invoke();
                Debug.Log($"<color=yellow>Auto-gán {item.itemName} → Hotbar slot {i + 1}</color>");
                return;
            }
        }
    }

    void UseActiveSlot()
    {
        ItemData item = slotItems[ActiveSlot];
        if (item == null)
        {
            Debug.Log("<color=grey>Slot trống!</color>");
            return;
        }

        if (item.itemType != ItemType.Consumable && item.itemType != ItemType.Potion)
        {
            Debug.Log("<color=grey>Item này không thể dùng từ hotbar!</color>");
            return;
        }

        // Check inventory còn item không
        if (!InventoryManager.Instance.HasItem(item))
        {
            Debug.Log($"<color=red>Hết {item.itemName}!</color>");
            ClearSlot(ActiveSlot);
            return;
        }

        // Tìm Player và dùng item
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (hp == null || stats == null) return;

        bool used = false;

        if (item.healAmount > 0)
        {
            if (hp.currentHP < stats.maxHP)
            {
                hp.currentHP = Mathf.Min(hp.currentHP + item.healAmount, stats.maxHP);
                used = true;
                Debug.Log($"<color=green>Dùng {item.itemName}! +{item.healAmount} HP</color>");
            }
            else
            {
                Debug.Log("<color=grey>Máu đã đầy!</color>");
            }
        }

        if (item.manaAmount > 0)
        {
            if (hp.currentMP < stats.maxMP)
            {
                hp.currentMP = Mathf.Min(hp.currentMP + item.manaAmount, stats.maxMP);
                used = true;
                Debug.Log($"<color=blue>Dùng {item.itemName}! +{item.manaAmount} MP</color>");
            }
            else
            {
                Debug.Log("<color=grey>Mana đã đầy!</color>");
            }
        }

        if (used)
        {
            InventoryManager.Instance.RemoveItem(item, 1);
            hp.UpdateAllUI();

            // Nếu hết item trong kho → xóa khỏi hotbar
            if (!InventoryManager.Instance.HasItem(item))
                ClearSlot(ActiveSlot);

            OnHotbarChanged?.Invoke();
        }
    }

    // === Được gọi bởi ItemPickup khi player tới gần / rời xa ===
    public void SetNearbyPickup(ItemPickup pickup)
    {
        _nearbyPickup = pickup;
    }

    public void ClearNearbyPickup(ItemPickup pickup)
    {
        if (_nearbyPickup == pickup)
            _nearbyPickup = null;
    }
}
