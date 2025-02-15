using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemPanel : ItemPanel
{
    public override void InsertItem(ItemIcon itemIcon)
    {
        if (itemIcon.dropItem ==null || !InventoryController.Instance.dropItemList.Contains(itemIcon.dropItem.GetComponent<DropItem>()))
        {
            InventoryController.Instance.CreateDropItem(itemIcon);
            InventoryController.Instance.dropItemList.Add(itemIcon.dropItem.GetComponent<DropItem>());
        }
        base.InsertItem(itemIcon);
    }
    public override void TakeOutItem(Item item)
    {
        /*if (InventoryController.Instance.dropItemList.Contains(item))
        {
            base.TakeOutItem(item);
            InventoryController.Instance.dropItemList.Remove(item);
            //InventoryController.Instance.RemoveDropItem(item);
        }*/
    }

    /*public void RemoveDropItem(Item item)
    {
        InventoryController.Instance.RemoveDropItem(item);
    }*/
    

    
}
