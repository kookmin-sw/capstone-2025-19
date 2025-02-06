using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] Transform itemIconParent;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] public InventoryController inventoryController;
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
        DropItemPanel dropItemPanel = itemIcon.itemPanel as DropItemPanel;
        if (dropItemPanel != null)
        {
            dropItemPanel.RemoveDropItem(itemIcon.item);
        }
        itemIcon.transform.SetParent(scrollRect.content);
        itemIcon.itemIconParent = itemIconParent;
        itemIcon.itemPanel = this;
    }
    public virtual void TakeOutItem(Item item)
    {
        //TODO ItemIcon setActive false 시키기, 혹은 지우기
    }
}
