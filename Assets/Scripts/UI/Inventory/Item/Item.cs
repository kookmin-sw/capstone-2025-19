using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    [HideInInspector]
    public GameObject dropItem = null;
    [HideInInspector]
    public GameObject itemIcon = null;

    [HideInInspector]
    public GameObject activeWeapon = null;

    public int quantity = 1;
    public float itemDurability = 1f;

    public void SetActiveTrueItemIcon()
    {
        itemIcon.SetActive(true);
        ItemIcon itemIcon_ = itemIcon.GetComponent<ItemIcon>();
        itemIcon_.item = this;
    }
    public void SetActiveFalseItemIcon()
    {
        ItemIcon itemIcon_ = itemIcon.GetComponent<ItemIcon>();
        //TODO 가방 안에 있는 아이템 떨구기
        /*if (inventoryItem.scrollView != null) { inventoryItem.scrollView.CloseContainer(); }
        inventoryItem.item = null;*/
        itemIcon.SetActive(false);
    }
    public void SetActiveTrueDropItem()
    {
        /*Collider collider = dropItem.GetComponent<Collider>();
        collider.enabled = true;
        DropItem dropItem_ = dropItem.GetComponent<DropItem>();
        dropItem_.SetItem(this);
        dropItem.SetActive(true);*/
        //dropItem_.SetSprite();
        DropItem dropItem_ = dropItem.GetComponent<DropItem>();
        dropItem_.SetItem(this);
        dropItem.SetActive(true);
    }
    public void SetActiveFalseDropItem()
    {
        /*Collider collider = dropItem.GetComponent<Collider>();
        collider.enabled = false;
        DropItem dropItem_ = dropItem.GetComponent<DropItem>();
        dropItem_.item = null;
        dropItem.SetActive(false);*/
        DropItem dropItem_ = dropItem.GetComponent<DropItem>();
        dropItem_.item = null;
        dropItem = null;
        dropItem_.DestoryItem();
        //dropItem.SetActive(false);
    }
    /*public void SetActiveFalseRPCDropItem()
    {
        
    }*/

    public void LoadItem(PlayerInventory.InventoryItem inventoryItem)
    {
        this.itemData = Resources.Load<ItemData>($"ItemData/{inventoryItem.itemName}");
        this.quantity = inventoryItem.quantity;
        this.itemDurability = inventoryItem.durability;
    }

}
