using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemPanel : ItemPanel
{
    public override void InsertItem(ItemIcon itemIcon)
    {
        if (!inventoryController.dropItemList.Contains(itemIcon.item))
        {
            inventoryController.dropItemList.Add(itemIcon.item);
            inventoryController.CreateDropItem(itemIcon.item);
            base.InsertItem(itemIcon);
        }
    }
    public override void TakeOutItem(Item item)
    {
        if (inventoryController.dropItemList.Contains(item))
        {
            base.TakeOutItem(item);
            inventoryController.dropItemList.Remove(item);
            inventoryController.RemoveDropItem(item);
        }
    }

    public void RemoveDropItem(Item item)
    {
        inventoryController.RemoveDropItem(item);
    }
    

    
}
