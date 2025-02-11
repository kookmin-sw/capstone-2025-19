using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
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
            //TODO �巡�� �ϰ� �ִ� ���� �ٸ� ����� �Ծ��ų� �������� �ָ� �������� �Ǿ��ų�
            return;
        }*/
        DropItemPanel dropItemPanel = itemIcon.itemPanel as DropItemPanel;
        DropItemPanel thisItemPanel = this as DropItemPanel;
        InventoryPanel inventoryPanel = itemIcon.itemPanel as InventoryPanel;
        InventoryPanel thiItemPanel = this as InventoryPanel;
        if (dropItemPanel != null && thisItemPanel == null)
        {
            InventoryController.Instance.RemoveDropItem(itemIcon.dropItem.GetComponent<DropItem>());
            //dropItemPanel.RemoveDropItem(itemIcon.item);
        }
        else if(inventoryPanel != null && thisItemPanel == null)
        {
            InventoryController.Instance.inventory.Remove(itemIcon.item);
        }
        itemIcon.transform.SetParent(scrollRect.content);
        itemIcon.itemPanel = this;
    }
    public virtual void TakeOutItem(Item item)
    {
        //TODO ItemIcon setActive false ��Ű��, Ȥ�� �����
    }
}
