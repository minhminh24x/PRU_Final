using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Currency,
    Consumable,
    Equipment
}

public enum ItemQuality
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public ItemType itemType;
    public ItemQuality quality;
    public Sprite icon;
    [TextArea] public string description;
    public int buyPrice;

    [Header("Consumable Stats")]
    public int healAmount;
    public int manaAmount;
    public float buffDamagePercent;
    public float buffDuration;

    [Header("Equipment Stats")]
    public int bonusDEF;
    public int bonusSTR;
    public int bonusINT;
    public int bonusAGI;
}
