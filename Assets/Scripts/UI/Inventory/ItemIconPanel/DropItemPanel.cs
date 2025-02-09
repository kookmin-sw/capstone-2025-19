using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemPanel : ItemPanel
{
    public override void InsertItem(ItemIcon itemIcon)
    {
        if (!InventoryController.Instance.dropItemList.Contains(itemIcon.item))
        {
            InventoryController.Instance.dropItemList.Add(itemIcon.item);
            InventoryController.Instance.CreateDropItem(itemIcon.item);
        }
        base.InsertItem(itemIcon);
    }
    public override void TakeOutItem(Item item)
    {
        if (InventoryController.Instance.dropItemList.Contains(item))
        {
            base.TakeOutItem(item);
            InventoryController.Instance.dropItemList.Remove(item);
            InventoryController.Instance.RemoveDropItem(item);
        }
    }

    /*public void RemoveDropItem(Item item)
    {
        InventoryController.Instance.RemoveDropItem(item);
    }*/
    

    
}
