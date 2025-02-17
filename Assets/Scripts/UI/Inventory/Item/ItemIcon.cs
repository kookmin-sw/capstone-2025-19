using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] Image itemImage;
    public Item item;
    public GameObject dropItem = null;
    public ItemPanel itemPanel;


    

    public void SetItem(Item item)
    {
        this.item = item;
        itemNameText.text = this.item.itemData.name;
        itemImage.sprite = this.item.itemData.itemIcon;
        Debug.Log($"ItemIcon SetItem {this.item.quantity} {this.item.durability}");
    }

    public void RemoveItemIcon()
    {
        dropItem = null;
        GetComponent<ItemIconInteract>().DestroyItemAlpha();
        Destroy(gameObject);
    }

    
}
[Serializable]
public class Item
{
    public ItemData itemData;
    public int quantity = 1;
    public float durability = 1f;

    /*    public GameObject itemIcon = null;
        public GameObject dropItem = null;*/


    public Item(ItemData itemData, int quantity, float durability)
    {
        this.itemData = itemData;
        this.quantity = quantity;
        this.durability = durability;
    }
}



