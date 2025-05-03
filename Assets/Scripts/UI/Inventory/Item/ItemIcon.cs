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
        this.item.itemIcon = this;
        itemNameText.text = this.item.itemData.name;
        itemImage.sprite = this.item.itemData.itemIcon;
        itemImageInfo.GetItem(item);
        SetSlider(this.item);
    }

    public void RemoveItemIcon()
    {
        GetComponent<ItemIconInteract>().DestroyItemAlpha();
        if (itemPanel != null) { itemPanel.RemoveItem(this); }
        dropItem = null;
        Destroy(gameObject);
    }
    private void SetSlider(Item item)
    {
        this.item = item;
        if(item.itemData.itemType == ItemData.ItemType.Weapon)
        {
            durabilitySlider.gameObject.SetActive(true);
            durabilitySlider.maxValue = this.item.itemData.maxItemDurability;
            durabilitySlider.value = this.item.durability;
        }
        else if(item.itemData.itemType == ItemData.ItemType.Potion)
        {
            durabilitySlider.gameObject.SetActive(true);
            durabilitySlider.maxValue = this.item.itemData.maxQuantity;
            durabilitySlider.value = this.item.quantity;
        }
        
        SetDurabilityColor(item.itemData);
        itemSliderInfo.GetItem(item);
    }
    public void SetSlider()
    {
        if(item.itemData.itemType == ItemData.ItemType.Potion)
        {
            durabilitySlider.value = this.item.quantity;
        }else if(item.itemData.itemType == ItemData.ItemType.Weapon)
        {
            durabilitySlider.value = this.item.durability;
        }
    }

    private void SetDurabilityColor(ItemData itemData)
    {
        // 100 ~ 66 green 66 ~ 33 orange 33 ~ 0 red
        float durabilityPercent = ((float)durabilitySlider.value / (float)itemData.maxQuantity) * 100;
        if(durabilityPercent >= 66)
        {
            fillImage.color = new Color32(84, 238, 106, 255);
        }else if(durabilityPercent >= 33)
        {
            fillImage.color = new Color32(227, 177, 78, 255); ;
        }
        else
        {
            fillImage.color = new Color32(184, 33, 35, 255);
        }
    }

    public bool PlusItemQuantity(ref Item item)
    {
        this.item.quantity += item.quantity;
        if(this.item.quantity > this.item.itemData.maxQuantity)
        {
            item.quantity = this.item.itemData.maxQuantity -= this.item.quantity;
            return false;
        }return true;
    }

    public void PlusItemDurability(float value)
    {
        this.item.durability += value;
        if(this.item.durability > item.itemData.maxItemDurability)this.item.durability = item.itemData.maxItemDurability;
        SetSlider();
    }
    
    
}
[Serializable]
public class Item
{
    public ItemIcon itemIcon;
    public ItemData itemData;
    public int quantity = 1;
    public float durability = 1f;

    


    public Item(ItemData itemData, int quantity, float durability)
    {
        this.itemData = itemData;
        this.quantity = quantity;
        this.durability = durability;
    }
    public float GetSize()
    {
        switch (this.itemData.itemType)
        {
            case ItemData.ItemType.Objects:
                return this.itemData.size * this.quantity;
            default:
                return this.itemData.size;
        }
    }
    public float GetWeight()
    {
        return this.itemData.Weight * this.quantity;
    }

    public void UseItem()
    {
        if (this.itemData.itemType == ItemData.ItemType.Potion)
        {
            foreach(ItemEffect effect in this.itemData.effectList)
            {
                if (!effect.Effect()) { quantity += 1; break; }
            }
            quantity -= 1;
            UpdateInfo();
        }
        else { Debug.LogError($"It is not Potion type it's{this.itemData.itemType}"); }
    }

    public void UpdateInfo()
    {
        if (durability <= 0) InventoryController.Instance.RemoveItemIcon(this.itemIcon);
        if (quantity <= 0) InventoryController.Instance.RemoveItemIcon(this.itemIcon);
        this.itemIcon.SetSlider();
    }

    
}



