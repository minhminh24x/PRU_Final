using UnityEngine;

public enum ItemType
{
    Consumable,   // Bình máu, mana, elixir
    Equipment,    // Giáp, mũ, giày, phụ kiện
    KeyItem       // Shard, quest item
}

public enum EquipSlot
{
    None,
    Helmet,
    Armor,
    Boots,
    Accessory
}

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemName = "Item";
    public Sprite icon;
    [TextArea] public string description = "";

    [Header("Phân loại")]
    public ItemType itemType = ItemType.Consumable;
    public EquipSlot equipSlot = EquipSlot.None;

    [Header("Stacking")]
    public bool stackable = true;
    public int maxStack = 5;

    [Header("Consumable Effects")]
    public int healAmount = 0;    // Hồi HP
    public int manaAmount = 0;    // Hồi MP
    public float buffDuration = 0f;
    public float buffDamagePercent = 0f; // +10% damage v.v.

    [Header("Equipment Bonus")]
    public int bonusDEF = 0;
    public int bonusSTR = 0;
    public int bonusINT = 0;
    public int bonusAGI = 0;

    [Header("Kinh tế")]
    public int buyPrice = 50;
    public int sellPrice = 10;
}
