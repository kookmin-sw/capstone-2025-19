using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryPanel : ItemPanel
{

    [SerializeField] Transform weaponParent;
    [SerializeField] Transform potionParent;
    [SerializeField] Transform objectParent;

    [SerializeField] Transform content;
    
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
        //TODO 아이템 parent 설정 다시 하기(넣으면 맨 위에 처박힘)
        /*switch (itemIcon.item.itemData.itemType_)
        {
            case ItemData.ItemType.Weapon:
                itemIcon.transform.parent = weaponParent; break;
            case ItemData.ItemType.Potion:
                itemIcon.transform.parent = potionParent; break;
            default:
                itemIcon.transform.parent = objectParent; break;
        }*/
        itemIcon.transform.parent = content.transform;
        itemIcon.itemPanel = this;

        InventoryController.Instance.inventory.Add(itemIcon.GetComponent<ItemIcon>().item);
        InventoryController.Instance.SetInventorySizeRate();
        Debug.Log($"Test InventoryController inventory {InventoryController.Instance.inventory.Count}");
        InventoryController.Instance.RemoveItemsUntilUnderMaxWeight();
        Debug.Log($"InsertItem count : {InventoryController.Instance.inventory.Count}");

        //QuestManager에게 알려주기
        Debug.Log($"In InsertItem {itemIcon}");
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

    public override void RemoveItem(ItemIcon itemIcon)
    {
        InventoryController.Instance.inventory.Remove(itemIcon.item);
        InventoryController.Instance.SetInventorySizeRate();
    }
}
