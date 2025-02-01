using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] TestPlayerMovement player;
    [SerializeField] GameObject itemIconPrefab;

    [SerializeField] InventoryPanel inventoryPanel;
    [SerializeField] DropItemPanel dropItemPanel;

    List<ItemIcon> inventoryList;
    public List<Item> dropItemList;
    [SerializeField] List<ItemData> itemDataList;

    [SerializeField] Transform itemIconParent;

    ItemPanel selectedItemPanel;

    DropItem testDropITem;
    public ItemPanel SelectedItemPanel { get =>  selectedItemPanel; set { selectedItemPanel = value; } }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        //TODO 아이템 저장하기 전에 new List 하기
        inventoryList = new List<ItemIcon>();
    }
    // Start is called before the first frame update
    void Start()
    {
        TestItemIcon();
    }

    // Update is called once per frame
    void Update()
    {
        /*if(testDropITem != null)
        {
            Debug.Log($"test {testDropITem.item.name}");
        }*/
    }

    public void EnableInventory()
    {
        canvasGroup.alpha = 1;
    }
    public void DisableInventory()
    {
        canvasGroup.alpha = 0;
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
            GameObject dropItemGo = Instantiate(Resources.Load<GameObject>($"Prefabs/Objects/DropItem/{item.itemData.name}"));
            dropItemGo.transform.position = player.transform.position; //아이템 버릴 곳
            DropItem dropItem = dropItemGo.GetComponent<DropItem>();
            dropItem.SetItem(item);
            testDropITem = dropItem;
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
        Debug.Log($"remove item test1 {item.dropItem}");

        if (item.dropItem != null)
        {
            Debug.Log($"remove item test2 {item.dropItem}");
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
        if (dropItem.item != null)
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
}
