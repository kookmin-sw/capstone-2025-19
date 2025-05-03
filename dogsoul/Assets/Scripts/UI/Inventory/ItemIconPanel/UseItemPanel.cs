using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UseItemPanel : ItemPanel
{

    ItemIcon useItemIcon;
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
        if (itemIcon.item.itemData.itemType != ItemData.ItemType.Potion){ return; }
        if (useItemIcon != null)
        {
            ChangeItem();
            //return; 
        }
        base.InsertItem(itemIcon);
        itemIcon.transform.SetParent(transform);
        useItemIcon = itemIcon;
        Debug.Log($"useItemPanel {itemIcon.item.itemData}");
        //TODO UseItem panel 에 적용
        
    }

    private void ChangeItem()
    {
        InventoryController.Instance.inventoryPanel.InsertItem(useItemIcon);
        useItemIcon = null;
    }

    public void UseItem()
    {
        if (useItemIcon != null)
        {
            useItemIcon.item.UseItem();
        }
        else { Debug.LogError($"No Items"); }
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        useItemIcon = null;
        //TODO UseItem slot 비우기
        
    }
}
