using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    GameObject purchaseButton;
    GameObject sellButton;
    bool storeType = true;
    List<StoreItemIcon> itemList = new List<StoreItemIcon>();
    int totalPrice = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InsertItem(StoreItemIcon item)
    {
        if(item.item != null) { totalPrice += item.item.itemData.price; }
        else { totalPrice += item.itemData.price; }
    }

    public void RemoveItemIcon(StoreItemIcon item)
    {
        if (item.item != null) { ItemIcon itemIcon = InventoryController.Instance.GetCreateItemIcon(item.item); InventoryController.Instance.inventoryPanel.InsertItem(itemIcon); 
            totalPrice -= item.item.itemData.price;
            item.item = null;
        }
        else { totalPrice -= item.itemData.price; }
        Destroy(item.gameObject);
        
    }

    public void SetItem(Item item)
    {

    }

    public void SetItem(StoreItemIcon item)
    {

    }

    public void PurchaseButton()
    {
        if(InventoryController.Instance.money < totalPrice)
        {
            //TODO 자금이 모자르다는 UI표시 혹은 버튼 비활성화
            Debug.LogError("Not enought money");
            return;
        }
        foreach(StoreItemIcon item in itemList)
        {

        }
    }
}
