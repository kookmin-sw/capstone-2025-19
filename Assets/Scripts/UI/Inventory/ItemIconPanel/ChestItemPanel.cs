using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItemPanel : ItemPanel
{
    WarehouseInteract warehaouse;

    public void OnEnable()
    {
        //TODO player의 상태가 Inventory_Chest 상태가 아니면 activeFalse;
        //gameObject.SetActive(false);
    }
    public override void InsertItem(ItemIcon itemIcon)
    {
        base.InsertItem(itemIcon);
        warehaouse.itemList.Add(itemIcon.item);
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        warehaouse.itemList.Remove(itemIcon.item);
    }

    public void SetWarehouse(WarehouseInteract warehaouse)
    {
        this.warehaouse = warehaouse;
    }

    public void ClearItemIcon()
    {
        foreach(GameObject itemIcon in scrollRect.transform)
        {
            itemIcon.GetComponent<ItemIcon>().RemoveItemIcon();
        }
    }
}
