using UnityEngine;

[System.Serializable]
public enum EquipSlot
{
    Helmet,
    Armor,
    Boots,
    Accessory
}

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
