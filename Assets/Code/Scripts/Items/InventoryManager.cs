using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    // --- Đã gỡ bỏ lớp lồng nhau InventorySlot để dùng lớp toàn cục trong SharedTypes.cs ---

    public List<InventorySlot> inventoryItems = new List<InventorySlot>();
    public int maxInventorySize = 20;

    // --- MỚI: API cho UI "Advanced" ---
    public List<InventorySlot> slots => inventoryItems;
    public int maxSlots => maxInventorySize;
    public event System.Action OnInventoryChanged; // ĐÃ SỬA: Bỏ static để instance access được

    public InventoryStaticUIController uiController;
    public List<ItemData> currencyItemDataList; // Kéo asset Coin, Gem,... vào Inspector

    private void Awake()
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
    /// Thêm item, nếu đã có thì cộng dồn số lượng, không thì add slot mới
    /// </summary>
    // Thêm vật phẩm vào kho (nếu vật phẩm đã có thì tăng số lượng)
    public bool AddItem(ItemData item, int amount = 1)
    {
        // Kiểm tra xem vật phẩm đã có trong kho chưa
        var existingSlot = inventoryItems.FirstOrDefault(slot => slot.item.itemName == item.itemName);  // Compare by itemName instead of the whole object

        if (existingSlot != null)
        {
            // Nếu vật phẩm đã có, tăng số lượng (stack)
            existingSlot.quantity += amount;
            Debug.Log($"Vật phẩm {item.itemName} đã có, cộng dồn {amount} vào kho, tổng số lượng: {existingSlot.quantity}");
        }
        else
        {
            // Kiểm tra giới hạn inventory (nếu có)
            if (inventoryItems.Count >= maxInventorySize)
            {
                Debug.LogWarning("Inventory full!");
                return false;
            }

            // Nếu vật phẩm chưa có, thêm một slot mới với số lượng là amount
            inventoryItems.Add(new InventorySlot(item, amount));
            // Thêm vật phẩm vào ItemDatabase
            ItemDatabase.AddItem(item);
            Debug.Log($"Thêm mới vật phẩm {item.itemName} vào kho với số lượng: {amount}");
        }
        // Gọi UI nếu có (null-check để tránh crash ở scene không có các manager này)
        if (PotionUI.Instance != null) PotionUI.Instance.UpdatePotionUI();

        // Cập nhật lại UI kho sau khi thêm vật phẩm
        if (InventoryStaticUIController.Instance != null)
            InventoryStaticUIController.Instance.UpdateInventorySlots();

        // Tự động gán consumable/potion vào hotbar nếu còn slot trống
        HotbarManager.Instance?.AutoAssign(item);

        // Lưu kho vào tệp JSON sau khi thay đổi
        InventoryFileHandler.SaveInventoryToFile(inventoryItems);
        
        OnInventoryChanged?.Invoke(); // Bắn event
        return true;
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        var slot = inventoryItems.FirstOrDefault(s => s.item.itemName == item.itemName);
        return slot != null && slot.quantity >= amount;
    }

    public int GetItemCount(ItemData item)
    {
        var slot = inventoryItems.FirstOrDefault(s => s.item.itemName == item.itemName);
        return slot != null ? slot.quantity : 0;
    }



    /// <summary>
    /// Xóa 1 số lượng item khỏi slot (ví dụ khi dùng hoặc vứt)
    // Xóa vật phẩm khỏi kho (giảm số lượng hoặc xóa hẳn nếu số lượng = 0)
    public void RemoveItem(ItemData item, int amount = 1)
    {
        Debug.Log("Phương thức Remove được gọi cho vật phẩm: " + item.itemName);
        var existingSlot = inventoryItems.FirstOrDefault(slot => slot.item.itemName == item.itemName);

        if (existingSlot != null)
        {
            existingSlot.quantity -= amount;

            // Nếu số lượng bằng 0, xóa vật phẩm khỏi kho
            if (existingSlot.quantity <= 0)
            {
                inventoryItems.Remove(existingSlot);
            }

            // Debug để kiểm tra số lượng
            Debug.Log($"Số lượng {item.itemName} còn lại trong inventory: {existingSlot.quantity}");

            // Cập nhật lại UI kho sau khi xóa vật phẩm
            if (InventoryStaticUIController.Instance != null)
                InventoryStaticUIController.Instance.UpdateInventorySlots();

            // Lưu lại inventory vào file JSON sau khi thay đổi
            InventoryFileHandler.SaveInventoryToFile(inventoryItems);
            OnInventoryChanged?.Invoke(); // Bắn event
        }
        else
        {
            Debug.LogWarning("Vật phẩm không có trong inventory.");
        }
    }

    private CurrencyData FindCurrencyItem(CurrencyType type)
    {
        foreach (var item in currencyItemDataList)
        {
            if (item is CurrencyData currencyItem && currencyItem.currencyType == type)
                return currencyItem;
        }
        return null;
    }


    public void AddCurrency(CurrencyType type, int amount)
    {
        Debug.Log($"Adding {amount} {type} to inventory.");
        // Tìm asset CurrencyData đúng loại
        CurrencyData currencyItem = FindCurrencyItem(type);
        if (currencyItem == null)
        {
            Debug.LogError("Chưa có asset CurrencyData cho loại tiền: " + type);
            return;
        }
        AddItem(currencyItem, amount);
    }

    public int GetCurrencyAmount(CurrencyType type)
    {
        foreach (var slot in inventoryItems)
        {
            if (slot.item is CurrencyData currencyItem && currencyItem.currencyType == type)
                return slot.quantity;
        }
        return 0;
    }
    public void ResetInventory()
    {
        inventoryItems.Clear();
        Debug.Log("[InventoryManager] Đã reset inventory.");
        // Cập nhật lại UI kho sau khi thêm vật phẩm
        if (InventoryStaticUIController.Instance != null)
            InventoryStaticUIController.Instance.UpdateInventorySlots();
    }


}