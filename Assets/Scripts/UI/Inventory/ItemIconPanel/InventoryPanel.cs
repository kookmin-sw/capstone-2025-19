using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : ItemPanel
{
    public override void InsertItem(ItemIcon itemIcon)
    {
        base.InsertItem(itemIcon);
        InventoryController.Instance.inventory.Add(itemIcon.GetComponent<ItemIcon>().item);
    }
}
