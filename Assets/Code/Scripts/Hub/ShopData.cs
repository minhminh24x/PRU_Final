using UnityEngine;

/// <summary>
/// Một mặt hàng trong shop: ItemData + giá mua (có thể override giá trong ItemData).
/// </summary>
[System.Serializable]
public class ShopEntry
{
    public ItemData item;
    [Tooltip("Giá mua. Nếu = 0 thì tự lấy từ ItemData.buyPrice")]
    public int overridePrice = 0;
    [Tooltip("Số lượng tối đa Player có thể mua. -1 = không giới hạn.")]
    public int stockLimit = -1;

    public int GetPrice() => overridePrice > 0 ? overridePrice : item.buyPrice;
}

/// <summary>
/// ScriptableObject chứa danh sách hàng hóa của một NPC Shop.
/// Right-click Project → Create > Game/Shop Data để tạo mới.
/// </summary>
[CreateAssetMenu(fileName = "New Shop", menuName = "Game/Shop Data")]
public class ShopData : ScriptableObject
{
    [Header("Thông tin Shop")]
    public string shopkeeperName = "Merchant";
    public Sprite shopkeeperPortrait;
    [TextArea(1, 3)]
    public string greetingLine = "Chào mừng! Tôi có thứ tốt cho cậu đây.";

    [Header("Danh sách hàng hóa")]
    public ShopEntry[] items;
}
