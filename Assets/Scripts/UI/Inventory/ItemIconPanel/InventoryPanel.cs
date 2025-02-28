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
        InventoryController.Instance.SetInventoryLoadRate();
        Debug.Log($"InsertItem count : {InventoryController.Instance.inventory.Count}");

        //QuestManager에게 알려주기
        Debug.Log("In InsertItem");
        //QuestManager.Instance.OnItemAcquired();
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        if(itemPanel != this)
        {
            InventoryController.Instance.inventory.Remove(itemIcon.item);
            InventoryController.Instance.SetInventoryLoadRate();
        }
    }
}
