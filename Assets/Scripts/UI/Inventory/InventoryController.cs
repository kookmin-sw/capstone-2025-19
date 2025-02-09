using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
    public List<Item> dropItemList;
    [SerializeField] List<ItemData> itemDataList;

    [SerializeField] public Transform itemIconParent;

    ItemPanel selectedItemPanel;
    ItemIcon selectedItemIcon;

    DropItem testDropITem;



    [HideInInspector]
    public List<Item> inventory = new List<Item>();
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
    }

    // Update is called once per frame
    void Update()
    {
        /*if(testDropITem != null)
        {
            Debug.Log($"test {testDropITem.item.name}");
        }*/
    }


    private void CreateItemIcon(Item item)
    {
        if (item.itemIcon == null)
        {
            GameObject itemIconGo = Instantiate(itemIconPrefab);
            itemIconGo.GetComponent<ItemIcon>().SetItem(item);
        }
        else
        {
            item.SetActiveTrueItemIcon();
        }
        
    }
    public void CreateDropItem(Item item)
    {
        if(item.dropItem != null)
        {
            item.SetActiveTrueDropItem();
        }
        else
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
    }

    private void TestItemIcon()
    {
        foreach (ItemData itemData in itemDataList)
        {
            ItemIcon itemIcon = Instantiate(itemIconPrefab).GetComponent<ItemIcon>();
            Item item = itemIcon.GetComponent<Item>();
            item.itemData = itemData;
            item.itemIcon = itemIcon.gameObject;
            itemIcon.SetItem(item);
            itemIcon.inventoryController = this;
            inventoryPanel.InsertItem(itemIcon);
        }
    }
    public void InsertItemToSelectedItemPanel(ItemIcon itemIcon)
    {
        if(selectedItemPanel == null) { return; }
        selectedItemPanel.InsertItem(itemIcon);
    }
    public void InsertDropItemPanel(DropItem dropItem)
    {
        if (dropItem.item.itemIcon == null)
        {
            CreateItemIcon(dropItem.item);
        }
        dropItemPanel.InsertItem(dropItem.item.itemIcon.GetComponent<ItemIcon>());
        //TODO ItemIcon 만들어서 dropItemPanel.InsertItem() 하기
    }
    private void TakeOutDropItemPanel(DropItem dropItem)
    {
        if(dropItem.item.itemIcon != null)
        {
            dropItemPanel.TakeOutItem(dropItem.item);
        }
    }

    public void ExitDropItem(DropItem dropItem)
    {
        Debug.Log(testDropITem);
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
            
        }
        Debug.Log("5");
    }
    public void EnterDropItem(DropItem dropItem)
    {
        if(dropItem.item != null)
        {
            if (!dropItemList.Contains(dropItem.item))
            {
                dropItemList.Add(dropItem.item);
                CreateItemIcon(dropItem.item);
                dropItemPanel.InsertItem(dropItem.item.itemIcon.GetComponent<ItemIcon>());
            }
        }
    }

    public void LoadInventoryItem(PlayerInventory.InventoryItem inventoryItem)
    {
        Debug.Log($"아이템 로드중... {inventoryItem.itemName}");
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
        if (player == null) { return; }
        if(player.state == TestPlayerMovement.PlayerState.Inventory)
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }
}
