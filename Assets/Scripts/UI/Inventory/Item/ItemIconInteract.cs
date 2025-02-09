using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemIcon))]
public class ItemIconInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    ItemIcon itemIcon;
    RectTransform rectTransform;
    [SerializeField] GameObject itemIconAlphaPrefab;
    

    void Awake()
    {
        itemIcon = GetComponent<ItemIcon>();
        //rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject itemIconAlpha = Instantiate(itemIconAlphaPrefab);
        ItemIconAlpha alpha = itemIconAlpha.GetComponent<ItemIconAlpha>();
        alpha.SetItem(itemIcon.item);
        rectTransform = itemIconAlpha.GetComponent<RectTransform>();
        itemIconAlpha.transform.SetParent(InventoryController.Instance.itemIconParent);
        itemIconAlpha.transform.position = transform.position;
        InventoryController.Instance.SelectedItemIcon = itemIcon;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        InventoryController.Instance.SelectedItemIcon = null;
        if(InventoryController.Instance.SelectedItemPanel != null)
        {
            InventoryController.Instance.SelectedItemPanel.InsertItem(itemIcon);
        }
        Destroy(rectTransform.gameObject);
        rectTransform = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO 더블 클릭이든 클릭 유지 이든. 아이템 분배 기능
    }

    void OnDisable()
    {
        if(rectTransform != null)
        {
            Destroy(rectTransform.gameObject);
            rectTransform = null;
        }
    }


}
