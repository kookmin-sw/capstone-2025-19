using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : ItemPanel
{

    [SerializeField] Transform weaponParent;
    [SerializeField] Transform potionParent;
    [SerializeField] Transform objectParent;
    
    public override void InsertItem(ItemIcon itemIcon)
    {
        //base.InsertItem(itemIcon);
        if (itemIcon.itemPanel != null)
        {
            itemIcon.itemPanel.TakeOutItem(this, itemIcon);
        }

        /*if (scrollRect != null)
        {
            itemIcon.transform.SetParent(scrollRect.content);
        }*/
        switch (itemIcon.item.itemData.itemType_)
        {
            case ItemData.ItemType.Weapon:
                itemIcon.transform.parent = weaponParent; break;
            case ItemData.ItemType.Potion:
                itemIcon.transform.parent = potionParent; break;
            default:
                itemIcon.transform.parent = objectParent; break;
        }
        itemIcon.itemPanel = this;

        InventoryController.Instance.inventory.Add(itemIcon.GetComponent<ItemIcon>().item);
        InventoryController.Instance.SetInventorySizeRate();
        InventoryController.Instance.RemoveItemsUntilUnderMaxWeight();
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
            InventoryController.Instance.SetInventorySizeRate();
        }
    }
}
