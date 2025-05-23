using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] protected ScrollRect scrollRect;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void InsertItem(ItemIcon itemIcon)
    {
        /*if (!itemIcon.gameObject.activeSelf)
        {
            //TODO 드래그 하고 있는 사이 다른 사람이 먹었거나 아이템이 멀리 떨어지게 되었거나
            return;
        }*/
        Debug.Log($"InsertTest 1");
        /*DropItemPanel dropItemPanel = itemIcon.itemPanel as DropItemPanel;
        DropItemPanel dropItemPanelTest = this.GetComponent<DropItemPanel>();*/
        if(itemIcon.itemPanel != null &&(itemIcon.itemPanel != this))
        {
            Debug.Log($"InsertTest 2");
            itemIcon.itemPanel.TakeOutItem(this, itemIcon);
        }
        Debug.Log($"InsertTest 3");
        /*DropItemPanel dropItemPanel = itemIcon.itemPanel as DropItemPanel;
        DropItemPanel thisItemPanel = this as DropItemPanel;
        InventoryPanel inventoryPanel = itemIcon.itemPanel as InventoryPanel;
        InventoryPanel thisItemPanel_ = this as InventoryPanel;
        if (dropItemPanel != null && thisItemPanel == null)
        {
            InventoryController.Instance.RemoveDropItem(itemIcon.dropItem.GetComponent<DropItem>());
            //dropItemPanel.RemoveDropItem(itemIcon.item);
        }
        else if(inventoryPanel != null && thisItemPanel_ == null)
        {
            InventoryController.Instance.inventory.Remove(itemIcon.item);
        }*/
        if (scrollRect != null)
        {
            itemIcon.transform.SetParent(scrollRect.content);
        }
        itemIcon.itemPanel = this;
    }
    public virtual void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        //TODO ItemIcon setActive false 시키기, 혹은 지우기
    }

    public virtual void RemoveItem(ItemIcon itemIcon)
    {

    }
}
