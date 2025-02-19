using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static WareHouseDB;

#if UNITY_EDITOR
using static UnityEditor.Progress;
#endif

public class InventoryController : Singleton<InventoryController>
{
    [SerializeField] CanvasGroup canvasGroup;
    public TestPlayerMovement player;
    [SerializeField] GameObject itemIconPrefab;

    [SerializeField] InventoryPanel inventoryPanel;
    [SerializeField] DropItemPanel dropItemPanel;

    List<ItemIcon> inventoryList;
    public List<DropItem> dropItemList;
    [SerializeField] List<ItemData> itemDataList;

    [SerializeField] public Transform itemIconParent;


    public PlayerTest playerTest;
    ItemPanel selectedItemPanel;
    ItemIcon selectedItemIcon;

    DropItem testDropITem;

    public TestID testID;

    [HideInInspector]
    public List<Item> inventory = new List<Item>();
    public int money = 0;
    [HideInInspector]
    public List<Item> wareHouse = new List<Item>();
    public ItemPanel SelectedItemPanel { get =>  selectedItemPanel; set { selectedItemPanel = value; } }
    public ItemIcon SelectedItemIcon { get => selectedItemIcon; set { selectedItemIcon = value; } }

    protected override void Awake()
    {
        //TODO ������ �����ϱ� ���� new List �ϱ�
        base.Awake();
        testID = new TestID();
    }
    // Start is called before the first frame update
    void Start()
    {
        TestItemIcon();
        SetInventoryCanvas();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(testDropITem != null)
        {
            Debug.Log($"test {testDropITem.item.name}");
        }*/
    }


    public void CreateItemIcon(Item item)
    {
        GameObject itemIconGo = Instantiate(itemIconPrefab);
        ItemIcon itemIcon = itemIconGo.GetComponent<ItemIcon>();
        itemIcon.SetItem(item);
    }
    public void CreateItemIcon(DropItem dropItem)
    {
        GameObject itemIconGo = Instantiate(itemIconPrefab);
        dropItem.itemIcon = itemIconGo;
        ItemIcon itemIcon = itemIconGo.GetComponent<ItemIcon>();
        itemIcon.dropItem = dropItem.gameObject;
        Item item = new Item(dropItem.itemData, dropItem.quantity, dropItem.durability);
        itemIcon.SetItem(item);
    }
    public void CreateDropItem(ItemIcon itemIcon)
    {
        GameObject dropItemGo;
        if (itemIcon.dropItem == null)
        {
            if(SceneController.Instance.GetCurrentSceneName() == "Village")
            {
                GameObject dropItemPrefab = Resources.Load<GameObject>($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}");
                dropItemGo = Instantiate(dropItemPrefab);
                Destroy(dropItemGo.GetComponent<PhotonRigidbodyView>());
                Destroy(dropItemGo.GetComponent<PhotonView>());
            }
            else
            {
                dropItemGo = PhotonNetwork.Instantiate($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}", playerTest.dropItemPosition.position, Quaternion.identity);
            }
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
        GameObject dropItemGo = PhotonNetwork.Instantiate($"Prefabs/Objects/DropItem/{itemIcon.item.itemData.name}", playerTest.dropItemPosition.position, Quaternion.identity);
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
        //dropItemGo.transform.position = player.transform.position; //������ ���� ��
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
        //TODO ItemIcon ���� dropItemPanel.InsertItem() �ϱ�*/
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

    public void LoadInventoryItem(PlayerInventory.InventoryItem inventoryItem)
    {
        Debug.Log($"������ �ε���... {inventoryItem.itemName}");
        //TODO create itemIcon
        //TODO insert InventoryPanel
    }

    public void LoadWareHouseItem(WareHouseDB.WareHouseItem wareHouseItem)
    {
        Debug.Log($"������ �ε���... {wareHouseItem.itemName}");
        //TODO create itemIcon
        //TODO insert InventoryPanel
    }

    public void SetPlayer(TestPlayerMovement player)
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

    public void SetTestClassID()
    {
        testID.ID += 1;
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
            // ������ �̸��� quest.target�� ������ Ȯ��
            if (item.itemData.name == itemName)
            {
                // quantity��ŭ ������
                totalCount += item.quantity;
            }
        }
        return totalCount;
    }

}
