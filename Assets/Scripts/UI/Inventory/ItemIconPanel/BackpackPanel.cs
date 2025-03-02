using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackPanel : ItemPanel
{
    [SerializeField] BackpackLoadRate loadRate;
    ItemIcon backpackitemIcon;

    public float defaultLoadValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void InsertItem(ItemIcon itemIcon)
    {
        if(itemIcon.item.itemData.itemType_ != ItemData.ItemType.Backpack) { return; }
        if (backpackitemIcon != null)
        {
            ChangeBackpack();
            return;
        }
        base.InsertItem(itemIcon);
        backpackitemIcon = itemIcon;
        SetBackpack();
    }

    public void ChangeBackpack()
    {
        //TODO change backpack
    }
    public void SetBackpack()
    {
        Debug.Log($"{InventoryController.Instance.currentInventoryItemSizeValue} {backpackitemIcon}");
        if(backpackitemIcon != null)
        {
            loadRate.SetValue(InventoryController.Instance.currentInventoryItemSizeValue, backpackitemIcon.item.itemData.containerValue);
            backpackitemIcon.transform.SetParent(transform);
            backpackitemIcon.transform.position = transform.position;
            loadRate.backpackName = backpackitemIcon.name;
        }
        else
        {
            loadRate.SetValue(InventoryController.Instance.currentInventoryItemSizeValue, defaultLoadValue);
            loadRate.backpackName = null;
        }

    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        backpackitemIcon = null;
        SetBackpack() ;
    }

    public void SetLoadRate(float currentValue)
    {

    }

    public ItemIcon GetItemIcon()
    {
        return backpackitemIcon;
    }

    public float GetContainerValue()
    {
        if (backpackitemIcon != null) { return backpackitemIcon.item.itemData.containerValue; }
        else { return defaultLoadValue; }
    }
}
