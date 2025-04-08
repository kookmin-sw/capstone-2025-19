using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WareHouseDB;

public class WarehouseInteract : InteractGo
{
    WareHouseDB wareHouseDB;
    public List<Item> itemList = new List<Item>();
    public void Start()
    {
        wareHouseDB = GetComponent<WareHouseDB>();
        StartAddItemList();
    }
    public override void InteractObject()
    {
        active = true;
        InventoryController.Instance.SetChestItemPanel(ref itemList, this);
    }
    public override void CloseInteract()
    {
        if (active)
        {
            InventoryController.Instance.DisableChestItempanel();
        }
        
    }

    private void InventoryItemToItem(WareHouseItem inventoryItem)
    {
        Item item = new Item(Resources.Load<ItemData>($"ItemData/{inventoryItem.itemName}"), inventoryItem.quantity, inventoryItem.durability);
        itemList.Add(item);
    }
    private void StartAddItemList()
    {
        foreach (WareHouseItem inventoryItem in wareHouseDB.wareHouseList)
        {
            InventoryItemToItem(inventoryItem);
        }
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Tab)) {
            //TODO player 상태 Idle로 변환
            InventoryController.Instance.DisableChestItempanel();
        }
    }


}
