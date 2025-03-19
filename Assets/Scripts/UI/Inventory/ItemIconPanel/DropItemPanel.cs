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
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        if(itemPanel != this)
        {
            InventoryController.Instance.RemoveDropItem(itemIcon.dropItem.GetComponent<DropItem>());
        }
        /*if (InventoryController.Instance.dropItemList.Contains(item))
        {
            base.TakeOutItem(item);
            InventoryController.Instance.dropItemList.Remove(item);
            //InventoryController.Instance.RemoveDropItem(item);
        }*/
    }

    public override void RemoveItem(ItemIcon itemIcon)
    {
        Debug.Log($"{itemIcon}");
        if(itemIcon == InventoryController.Instance.selectedItemIcon_) { Debug.LogError("Who is change itemIcon dropItem"); }
        Debug.Log($"{itemIcon.dropItem}");
        InventoryController.Instance.dropItemList.Remove(itemIcon.dropItem.GetComponent<DropItem>());
    }

    public void InsertMoney(GameObject itemIcon)
    {
        itemIcon.transform.SetParent(scrollRect.content);
    }

    /*public void RemoveDropItem(Item item)
    {
        InventoryController.Instance.RemoveDropItem(item);
    }*/



}
