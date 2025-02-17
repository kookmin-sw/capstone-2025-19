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
        Debug.Log($"InsertItem count : {InventoryController.Instance.inventory.Count}");

        //QuestManager에게 알려주기
        Debug.Log("In InsertItem");
        QuestManager.Instance.OnItemAcquired();
    }
}
