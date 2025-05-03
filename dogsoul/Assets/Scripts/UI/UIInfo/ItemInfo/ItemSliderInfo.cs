using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSliderInfo : UIInfoData
{
    Item item;
    public override string GetContent()
    {
        switch (item.itemData.itemType)
        {
            case ItemData.ItemType.Weapon:
                return $"{item.itemData.name} Durability \n durability / max durability \n {item.durability} / {item.itemData.maxItemDurability}";
            case ItemData.ItemType.Potion:
                return $"{item.itemData.name} Quantity \n Quantity / Max Quantity \n {item.quantity} / {item.itemData.maxQuantity}";
            default:
                return null;
        }
    }

    public void GetItem(Item item)
    {
        this.item = item;
    }

    // Start is called before the first frame update
    
}
