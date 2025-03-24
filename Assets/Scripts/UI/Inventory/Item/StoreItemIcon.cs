using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class StoreItemIcon : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemPrice;
    public bool isPurchasePanel = false;
    public bool isInventoryItem = false;
    public ItemData itemData;
    public Item item = null;

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
        itemImage.sprite = this.itemData.itemIcon;
        itemName.text = this.itemData.name;
        itemPrice.text = this.itemData.price.ToString();
        isInventoryItem = false;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        itemImage.sprite = item.itemData.itemIcon;
        itemName.text = this.item.itemData.name;
        itemPrice.text = this.item.itemData.price.ToString();
        isInventoryItem = true;
    }
}
