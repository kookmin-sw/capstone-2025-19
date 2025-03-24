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
        if (itemIcon.isPurchasePanel)
        {
            //TODO Return item
            if (itemIcon.isInventoryItem) { Debug.Log($"itemIcon {itemIcon.item}"); } 
            else { InventoryController.Instance.purchasePanel.RemoveItemIcon(itemIcon); }
        }
        else
        {
            if(itemIcon.isInventoryItem) { Debug.Log($"itemIcon {itemIcon.item}"); }
            else { InventoryController.Instance.purchasePanel.SetItem(itemIcon);  }

        }
    }

    void Awake()
    {
        itemIcon = GetComponent<StoreItemIcon>();
    }
    
}
