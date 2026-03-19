using System;
using UnityEngine;

/// <summary>
/// Quản lý 4 slot trang bị: Helmet, Armor, Boots, Accessory.
/// Khi mặc đồ → cộng bonus stats vào PlayerStats.
/// Khi tháo đồ → trừ bonus stats, trả item về Inventory.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    // Mỗi slot chứa 1 ItemData (hoặc null)
    public ItemData helmet;
    public ItemData armor;
    public ItemData boots;
    public ItemData accessory;

    public event Action OnEquipmentChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Mặc đồ vào slot tương ứng.
    /// Nếu slot đã có đồ → tháo ra trước rồi mặc đồ mới.
    /// </summary>
    public bool Equip(ItemData item)
    {
        if (item == null || item.itemType != ItemType.Equipment) return false;
        if (item.equipSlot == EquipSlot.None) return false;

        // Tháo đồ cũ nếu có
        Unequip(item.equipSlot);

        // Xóa item khỏi inventory
        InventoryManager.Instance.RemoveItem(item, 1);

        // Gắn vào slot
        SetSlot(item.equipSlot, item);

        // Cộng stats
        ApplyBonus(item, true);

        Debug.Log($"<color=cyan>Mặc {item.itemName}!</color>");
        OnEquipmentChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Tháo đồ ra khỏi slot, trả về inventory.
    /// </summary>
    public void Unequip(EquipSlot slot)
    {
        ItemData current = GetSlot(slot);
        if (current == null) return;

        // Trừ stats
        ApplyBonus(current, false);

        // Trả về inventory
        InventoryManager.Instance.AddItem(current, 1);

        // Xóa slot
        SetSlot(slot, null);

        Debug.Log($"<color=orange>Tháo {current.itemName}</color>");
        OnEquipmentChanged?.Invoke();
    }

    public ItemData GetSlot(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.Helmet: return helmet;
            case EquipSlot.Armor: return armor;
            case EquipSlot.Boots: return boots;
            case EquipSlot.Accessory: return accessory;
            default: return null;
        }
    }

    void SetSlot(EquipSlot slot, ItemData item)
    {
        switch (slot)
        {
            case EquipSlot.Helmet: helmet = item; break;
            case EquipSlot.Armor: armor = item; break;
            case EquipSlot.Boots: boots = item; break;
            case EquipSlot.Accessory: accessory = item; break;
        }
    }

    void ApplyBonus(ItemData item, bool add)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        int sign = add ? 1 : -1;
        stats.extraDEF += item.bonusDEF * sign;
        stats.extraSTR += item.bonusSTR * sign;
        stats.extraINT += item.bonusINT * sign;
        stats.extraAGI += item.bonusAGI * sign;

        stats.Recalculate();

        // Cập nhật UI máu/mana
        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null) hp.UpdateAllUI();
    }
}
