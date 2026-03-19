using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Một ô trong kho đồ: chứa ItemData + số lượng.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

/// <summary>
/// Singleton quản lý toàn bộ kho đồ. DontDestroyOnLoad.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Cấu hình")]
    public int maxSlots = 20;

    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

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
    /// Thêm item vào kho. Trả về true nếu thêm được.
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // Nếu stackable, tìm slot có sẵn item cùng loại
        if (item.stackable)
        {
            InventorySlot existing = slots.Find(s => s.item == item && s.quantity < item.maxStack);
            if (existing != null)
            {
                int canAdd = Mathf.Min(amount, item.maxStack - existing.quantity);
                existing.quantity += canAdd;
                amount -= canAdd;

                if (amount <= 0)
                {
                    Debug.Log($"<color=green>+{canAdd} {item.itemName} (tổng: {existing.quantity})</color>");
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        // Tạo slot mới
        if (slots.Count >= maxSlots)
        {
            Debug.Log("<color=red>Kho đồ đầy!</color>");
            return false;
        }

        int addAmount = item.stackable ? Mathf.Min(amount, item.maxStack) : 1;
        slots.Add(new InventorySlot(item, addAmount));
        Debug.Log($"<color=green>+{addAmount} {item.itemName}</color>");
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Xóa item khỏi kho. Trả về true nếu đủ để xóa.
    /// </summary>
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        InventorySlot slot = slots.Find(s => s.item == item);
        if (slot == null || slot.quantity < amount) return false;

        slot.quantity -= amount;
        if (slot.quantity <= 0)
            slots.Remove(slot);

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Đếm tổng số lượng của một item trong kho.
    /// </summary>
    public int GetItemCount(ItemData item)
    {
        int total = 0;
        foreach (var s in slots)
        {
            if (s.item == item) total += s.quantity;
        }
        return total;
    }

    public bool HasItem(ItemData item)
    {
        return GetItemCount(item) > 0;
    }

    /// <summary>
    /// Tìm slot chứa item cụ thể.
    /// </summary>
    public InventorySlot FindSlot(ItemData item)
    {
        return slots.Find(s => s.item == item);
    }
}
