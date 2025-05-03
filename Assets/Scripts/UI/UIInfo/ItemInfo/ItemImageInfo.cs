using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemImageInfo : UIInfoData
{
    Item item;
    
    public override string GetContent()
    {
        switch (item.itemData.itemType)
        {
            case ItemData.ItemType.Weapon:
                return $"weapon status \nName : {item.itemData.name} \nStrength : attackDamage \nWeight : {item.itemData.Weight} \nDurability : {item.durability} / {item.itemData.maxItemDurability}";
            case ItemData.ItemType.Armor:
                return "Armor status \nName : {item.itemData.name} \nArmor Point : {armorPoint} \nWeight : {item.itemData.weight} \nDurability : {item.durability} / {item.itemData.maxDurability}";
            case ItemData.ItemType.Potion:
                return $"Use Item \nName : {item.itemData.name} \nItemEffect :  \nQuantity : {item.quantity} / {item.itemData.maxQuantity}";
            case ItemData.ItemType.Backpack:
                return "Container \nName : {item.itemData.name} \nLoadValue : {item.itemData.containerValue}";
            default:
                return null;
        }
    }
    public void GetItem(Item item)
    {
        this.item = item;
    }
}
