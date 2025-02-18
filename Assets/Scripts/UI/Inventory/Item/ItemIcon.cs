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
    [SerializeField] Slider durabilitySlider;
    [SerializeField] Image fillImage;
    [SerializeField] ItemImageInfo itemImageInfo;
    [SerializeField] ItemSliderInfo itemSliderInfo;
    public Item item;
    public GameObject dropItem = null;
    public ItemPanel itemPanel;

    //Color greenColor = ;
    //Color orangeColor = 
    //Color redColor = ;
    

    public void SetItem(Item item)
    {
        this.item = item;
        Debug.Log($"item {item}");
        Debug.Log($"this.item{this.item.itemData.name}");
        this.item.itemIcon = this;
        itemNameText.text = this.item.itemData.name;
        itemImage.sprite = this.item.itemData.itemIcon;
        itemImageInfo.GetItem(item);
        SetSlider(this.item);
        Debug.Log($"ItemIcon SetItem {this.item.quantity} {this.item.durability}");
    }

    public void RemoveItemIcon()
    {
        dropItem = null;
        GetComponent<ItemIconInteract>().DestroyItemAlpha();
        Destroy(gameObject);
    }
    private void SetSlider(Item item)
    {
        if (item.itemData.maxItemDurability == 1) { durabilitySlider.gameObject.SetActive(false); }
        else { durabilitySlider.gameObject.SetActive(true); }
        durabilitySlider.maxValue = this.item.itemData.maxQuantity;
        durabilitySlider.value = this.item.durability;
        SetDurabilityColor(item.itemData);
        itemSliderInfo.GetItem(item);
    }

    private void SetDurabilityColor(ItemData itemData)
    {
        // 100 ~ 66 green 66 ~ 33 orange 33 ~ 0 red
        float durabilityPercent = ((float)durabilitySlider.value / (float)itemData.maxQuantity) * 100;
        Debug.Log(durabilityPercent);
        if(durabilityPercent >= 66)
        {
            Debug.Log(durabilityPercent);
            fillImage.color = new Color32(84, 238, 106, 255);
        }else if(durabilityPercent >= 33)
        {
            Debug.Log(durabilityPercent);
            fillImage.color = new Color32(227, 177, 78, 255); ;
        }
        else
        {
            Debug.Log(durabilityPercent);
            fillImage.color = new Color32(184, 33, 35, 255);
        }
    }
    
}
[Serializable]
public class Item
{
    public ItemIcon itemIcon;
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



