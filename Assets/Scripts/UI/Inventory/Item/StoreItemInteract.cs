using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(StoreItemIcon))]
public class StoreItemInteract : MonoBehaviour,IPointerClickHandler
{
    RectTransform rectTransform;
    StoreItemIcon itemIcon;

  

    public void OnPointerClick(PointerEventData eventData)
    {
        if(itemIcon.item != null) { }
        else { InventoryController.Instance.purchasePanel.SetItem(itemIcon); }
    }

    void Awake()
    {
        itemIcon = GetComponent<StoreItemIcon>();
    }
    
}
