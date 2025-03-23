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
    public ItemData itemData;
    public Item item;

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
        itemImage.sprite = this.itemData.itemIcon;
        itemName.text = this.itemData.name;
        itemPrice.text = this.itemData.price.ToString();
    }
}
