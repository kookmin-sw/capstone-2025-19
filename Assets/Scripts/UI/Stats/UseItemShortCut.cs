using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UseItemShortCut : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Slider slider;
    Item item;

    public void SetItem(Item item)
    {
        Debug.Log($"use item test {item} asdfasdf");
        if(item == null) { image.gameObject.SetActive(false); slider.gameObject.SetActive(false); return; }
        image.gameObject.SetActive(true);
        slider.gameObject.SetActive(true);  
        this.item = item;
        image.sprite = this.item.itemData.itemIcon;
        slider.maxValue = this.item.itemData.maxQuantity;
        slider.value = this.item.quantity;
    }

    public void SetSlider()
    {
        if(item == null) { return; }
        slider.value = item.quantity;
    }

    public bool UseItem()
    {
        if (item== null)
        {
            return false;
        }
        item.UseItem();
        return true;
    }
}
