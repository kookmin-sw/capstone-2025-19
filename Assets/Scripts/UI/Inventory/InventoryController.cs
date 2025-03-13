using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static WareHouseDB;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class InventoryController : Singleton<InventoryController>
{
    [SerializeField] CanvasGroup canvasGroup;
    //public TestPlayerMovement player;
    //public PlayerManager player;
    public PlayerTrigger player;
    [SerializeField] GameObject itemIconPrefab;

    [SerializeField] public InventoryPanel inventoryPanel;
    [SerializeField] public DropItemPanel dropItemPanel;
    [SerializeField] public BackpackPanel backpackPanel;
    [SerializeField] public ChestItemPanel chestItemPanel;
    [SerializeField] public Weaponpanel weaponPanel;

    [SerializeField] public DistributionPanel distributionPanel;

    List<ItemIcon> inventoryList;
    public List<DropItem> dropItemList;
    [SerializeField] List<ItemData> itemDataList;

    [SerializeField] public Transform itemIconParent;
    [SerializeField] Slider inventoryLoadRateSlider;

    [SerializeField] public Transform popupParent;


    public PlayerTest playerTest;
    ItemPanel selectedItemPanel;
    ItemIcon selectedItemIcon;

    DropItem testDropITem;


    [HideInInspector]
    public List<Item> inventory = new List<Item>();
    [HideInInspector]
    public float currentInventoryWeightValue = 0f;
    public float currentInventoryItemSizeValue = 0f;

    public int money = 0;
    [HideInInspector]
    public List<Item> wareHouse = new List<Item>();
    public ItemPanel SelectedItemPanel { get =>  selectedItemPanel; set { selectedItemPanel = value; } }
    public ItemIcon SelectedItemIcon { get => selectedItemIcon; set { selectedItemIcon = value; } }

    protected override void Awake()
    {
        //TODO 아이템 저장하기 전에 new List 하기
        base.Awake();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        TestItemIcon();
        SetInventoryCanvas();
        SetInventorySizeRate();
    }

    public void SetInventorySizeRate()
    {
        
        foreach(Item item in inventory)
        {

            /*if (currentInventoryItemSizeValue + item.quantity * item.itemData.size > backpackPanel.GetContainerValue())
            {
                //TODO 넣을 수 있는 만큼 넣기 Drop item
                if(item.itemData.itemType_ == ItemData.ItemType.Objects)
                {
                    dropItemPanel.InsertItem(item.itemIcon);
                }
                
                continue;
            }*/
            currentInventoryItemSizeValue += item.quantity * item.itemData.size;
        }
        backpackPanel.SetBackpack();
    }
    public void RemoveItemsUntilUnderMaxWeight()
    {
        int tryCount = 0;
        while(currentInventoryItemSizeValue < backpackPanel.GetContainerValue())
        {
            int index = inventory.Count-1;
            if (inventory[index].itemData.itemType_ == ItemData.ItemType.Objects)
            {
                if (backpackPanel.GetContainerValue() < currentInventoryItemSizeValue - inventory[index].GetSize())
                {
                    currentInventoryItemSizeValue -= inventory[index].GetSize();
                    currentInventoryWeightValue -= inventory[index].GetWeight();
                    dropItemPanel.InsertItem(inventory[index].itemIcon);
                }
                else
                {
                    float value = currentInventoryItemSizeValue - backpackPanel.GetContainerValue();
                    int count = (int)(value / inventory[index].GetSize()) + 1;
                    Item item = new Item(inventory[index].itemData, count, 1);
                    ItemIcon itemIcon = GetCreateItemIcon(item);
                    dropItemPanel.InsertItem(itemIcon);
                    inventory[index].quantity -= count;
                    inventory[index].itemIcon.GetComponent<ItemIcon>().SetSlider();
                }
            }
            else
            {
                currentInventoryItemSizeValue -= inventory[index].GetSize();
                currentInventoryWeightValue -= inventory[index].GetWeight();
                dropItemPanel.InsertItem(inventory[index].itemIcon);
            }


            tryCount++;
            if(tryCount > 100) { Debug.LogError("trycount 100 over");break; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateItemIcon(Item item)
    {
        GameObject itemIconGo = Instantiate(itemIconPrefab);
        ItemIcon itemIcon = itemIconGo.GetComponent<ItemIcon>();
        itemIcon.SetItem(item);
    }
    public ItemIcon GetCreateItemIcon(Item item)
    {
        GameObject itemIconGo = Instantiate(itemIconPrefab);
        ItemIcon itemIcon = itemIconGo.GetComponent<ItemIcon>();
        itemIcon.SetItem(item);
        return itemIcon;
    }
    public void CreateItemIcon(DropItem dropItem)
    {
        GameObject itemIconGo = Instantiate(itemIconPrefab);
        dropItem.itemIcon = itemIconGo;
        ItemIcon itemIcon = itemIconGo.GetComponent<ItemIcon>();
        itemIcon.dropItem = dropItem.gameObject;
        Item item = new Item(dropItem.itemData, dropItem.quantity, dropItem.durability);
        itemIcon.SetItem(item);
        itemIconGo.name = $"{item.itemData.name}_ItemIcon";
    }
    public void CreateDropItem(ItemIcon itemIcon)
    {
        GameObject dropItemGo;
        if (itemIcon.dropItem == null)
        {
            if(SceneController.Instance.GetCurrentSceneName() != "Dungeon_Multiplay")
            {
                GameObject dropItemPrefab = Resources.Load<GameObject>($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}_DropItem");
                dropItemGo = Instantiate(dropItemPrefab);
                dropItemGo.name = $"{itemIcon.item.itemData.name}_DropItem";
                dropItemGo.transform.position = player.dropItemPosition.position;
                Destroy(dropItemGo.GetComponent<PhotonRigidbodyView>());
                Destroy(dropItemGo.GetComponent<PhotonView>());
            }
            else
            {
                dropItemGo = PhotonNetwork.Instantiate($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}_DropItem", playerTest.dropItemPosition.position, Quaternion.identity);
            }
            dropItemGo.name = $"{itemIcon.item.itemData.name}_DropItem";
            itemIcon.dropItem = dropItemGo;
            DropItem dropItem = dropItemGo.GetComponent<DropItem>();
            dropItem.itemIcon = itemIcon.gameObject;
            dropItem.SetItem(itemIcon.item);
        }
        else
        {
            dropItemGo = itemIcon.dropItem.gameObject;
            DropItem dropItem = dropItemGo.GetComponent<DropItem>();
            dropItem.itemIcon = itemIcon.gameObject;
            dropItem.SetItem(itemIcon.item);
            dropItem.ActivateDropItem(player.dropItemPosition);
        }

    }
    public void SetDropItemToItemIcon(ItemIcon itemIcon)
    {
        GameObject dropItemGo = PhotonNetwork.Instantiate($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}_DropItem", playerTest.dropItemPosition.position, Quaternion.identity);
        DropItem dropItem = dropItemGo.GetComponent<DropItem>();
        dropItem.SetItem(itemIcon.item);
        dropItem.itemIcon = itemIcon.gameObject;
    }

    public void RemoveItemIcon(ItemIcon itemIcon)
    {
        itemIcon.RemoveItemIcon();
    }
    public void RemoveItemIcon(DropItem dropItem)
    {
        if (dropItem.itemIcon == null) { return; }
        dropItem.itemIcon.GetComponent<ItemIcon>().RemoveItemIcon();
    }
    public void RemoveDropItem(DropItem dropItem)
    {
        dropItem.RemoveDropItem();
    }

    
    
    

    /*public void CreateDropItem_(Item item)
    {
        GameObject dropItemGo = PhotonNetwork.Instantiate($"Prefabs/Objects/DropItem/{item.itemData.name}", player.dropItemPosition.position, Quaternion.identity);
        //GameObject dropItemGo = Instantiate(Resources.Load<GameObject>($"Prefabs/Objects/DropItem/{item.itemData.name}"));
        //dropItemGo.transform.position = player.transform.position; //아이템 버릴 곳
        Debug.Log($"test dropItem Create {item.itemData}");
        DropItem dropItem = dropItemGo.GetComponent<DropItem>();
        dropItem.SetItem(item);
        testDropITem = dropItem;
        Debug.Log($"create dropItem {dropItem.item}");
    }
    public void RemoveItemIcon(Item item)
    {
        if(item.itemIcon != null)
        {
            ItemIcon itemIcon = item.itemIcon.GetComponent<ItemIcon>();
            item.SetActiveFalseItemIcon();
        }
    }
    public void RemoveDropItem(Item item)
    {
        dropItemList.Remove(item);
        if (item.dropItem != null)
        {
            item.SetActiveFalseDropItem();
        }
    }*/

    private void TestItemIcon()
    {
        foreach (ItemData itemData in itemDataList)
        {
            /*ItemIcon itemIcon = Instantiate(itemIconPrefab).GetComponent<ItemIcon>();
            Item item = itemIcon.GetComponent<Item>();
            item.itemData = itemData;
            item.itemIcon = itemIcon.gameObject;
            itemIcon.SetItem(item);
            itemIcon.inventoryController = this;
            inventoryPanel.InsertItem(itemIcon);*/
        }
    }
    public void InsertItemToSelectedItemPanel(ItemIcon itemIcon)
    {
        if(selectedItemPanel == null) { return; }
        selectedItemPanel.InsertItem(itemIcon);
    }
    public void InsertDropItemPanel(ItemIcon itemIcon)
    {
        dropItemPanel.InsertItem(itemIcon);
        /*if (dropItem.item.itemIcon == null)
        {
            CreateItemIcon(dropItem.item);
        }
        dropItemPanel.InsertItem(dropItem.item.itemIcon.GetComponent<ItemIcon>());
        //TODO ItemIcon 만들어서 dropItemPanel.InsertItem() 하기*/
    }
    private void TakeOutDropItemPanel(DropItem dropItem)
    {
        /*if(dropItem.item.itemIcon != null)
        {
            dropItemPanel.TakeOutItem(dropItem.item);
        }*/
    }

    public void ExitDropItem(DropItem dropItem)
    {
        Debug.Log("1");
        if(dropItem == null) { return; }
        Debug.Log("2");
        if (dropItemList.Contains(dropItem))
        {
            Debug.Log("3");
            dropItemList.Remove(dropItem);
            RemoveItemIcon(dropItem);
        }
        else
        {
            Debug.Log("4");
            RemoveDropItem(dropItem);
        }
        /*Debug.Log(testDropITem);
        Debug.Log(testDropITem.item);
        if (dropItem.item?.itemData != null)
        {
            if (dropItemList.Contains(dropItem.item))
            {
                dropItemList.Remove(dropItem.item);
                RemoveItemIcon(dropItem.item);
            }
            else
            {
                RemoveDropItem(dropItem.item);
            }
            
        }*/
    }
    public void EnterDropItem(DropItem dropItem)
    {
        if(dropItem == null) { return; }
        if(!dropItemList.Contains(dropItem))
        {
            dropItemList.Add(dropItem);
            CreateItemIcon(dropItem);
            InsertDropItemPanel(dropItem.itemIcon.GetComponent<ItemIcon>());
        }
        /*if(dropItem.item != null)
        {
            if (!dropItemList.Contains(dropItem.item))
            {
                dropItemList.Add(dropItem.item);
                CreateItemIcon(dropItem.item);
                dropItemPanel.InsertItem(dropItem.item.itemIcon.GetComponent<ItemIcon>());
            }
        }*/
    }

    public void LoadInventoryItem(PlayerInventoryDB.InventoryItem inventoryItem)
    {
        Debug.Log($"아이템 로드중... {inventoryItem.itemName}");
        //TODO create itemIcon
        //TODO insert InventoryPanel
    }

    public void LoadWareHouseItem(WareHouseDB.WareHouseItem wareHouseItem)
    {
        Debug.Log($"아이템 로드중... {wareHouseItem.itemName}");
        //TODO create itemIcon
        //TODO insert InventoryPanel
    }

    public void SetPlayer(PlayerTrigger player)
    {
        if(this.player == null)
        {
            this.player = player;
            SetInventoryCanvas();
        }
    }

    public void SetInventoryCanvas()
    {
        //if (player == null) { return; }
        if(PlayerState.Instance.state == PlayerState.State.Inventory)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

   

    public void UpDurability()
    {
        if (inventory.Count <= 0) { return; }
        Item item = inventory[0];
        item.durability += 1;
        DebugText.Instance.Debug($"{item.itemData.name} durability is {item.durability}");
    }

    public void UpQuantity()
    {
        if (inventory.Count <= 0) { return; }
        Item item = inventory[0];
        item.quantity += 1;
        DebugText.Instance.Debug($"{item.itemData.name} quantity is {item.quantity}");
    }

    //Util
    public int GetItemCount(string itemName)
    {
        int totalCount = 0;
        foreach (Item item in inventory)
        {
            // 아이템 이름이 quest.target과 같은지 확인
            if (item.itemData.name == itemName)
            {
                // quantity만큼 더해줌
                totalCount += item.quantity;
            }
        }
        return totalCount;
    }

    public void SetChestItemPanel(ref List<Item> itemList, WarehouseInteract warehouse)
    {
        //TODO player의 상태 Inventory_Chest로 변경
        
        chestItemPanel.gameObject.SetActive(true);
        chestItemPanel.SetWarehouse(warehouse);
        foreach(Item item in itemList)
        {
            CreateItemIcon(item);
            chestItemPanel.InsertItem(item.itemIcon);
        }
    }

    public void DisableChestItempanel()
    {
        //TODO player의 상태 Idle로 변경

    }

    public void SetPlayerInventory()
    {
        //Scene 이동 시 Inventory 내에서 적용 해야 할 아이템 적용 시키기
        weaponPanel.SetWeapon();
    }

}
