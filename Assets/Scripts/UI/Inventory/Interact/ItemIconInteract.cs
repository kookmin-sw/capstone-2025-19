using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemIcon))]
public class ItemIconInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    ItemIcon itemIconSlot;
    RectTransform rectTransform;
    [SerializeField] GameObject itemIconAlphaPrefab;
    

    void Awake()
    {
        itemIconSlot = GetComponent<ItemIcon>();
        //rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject itemIconAlpha = Instantiate(itemIconAlphaPrefab);
        ItemIconAlpha alpha = itemIconAlpha.GetComponent<ItemIconAlpha>();
        alpha.SetItem(itemIconSlot.item);
        rectTransform = itemIconAlpha.GetComponent<RectTransform>();
        itemIconAlpha.transform.SetParent(itemIconSlot.itemIconParent);
        itemIconAlpha.transform.position = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(itemIconSlot.inventoryController != null)
        {
            itemIconSlot.inventoryController.InsertItemToSelectedItemPanel(itemIconSlot);
        }
        Destroy(rectTransform.gameObject);
        rectTransform = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO 더블 클릭이든 클릭 유지 이든. 아이템 분배 기능
    }
}
