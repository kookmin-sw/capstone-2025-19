using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(ItemIcon))]
public class ItemIconInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    ItemIcon itemIcon;
    RectTransform rectTransform;
    [SerializeField] GameObject itemIconAlphaPrefab;
    GameObject itemIconAlpha;

    void Awake()
    {
        itemIcon = GetComponent<ItemIcon>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(PlayerState.Instance.state == PlayerState.State.Inventory)
        {
            rectTransform = Instantiate(itemIconAlphaPrefab, InventoryController.Instance.itemIconParent).GetComponent<RectTransform>();
            rectTransform.transform.position = transform.position;
            ItemIconAlpha alpha = rectTransform.GetComponent<ItemIconAlpha>();
            alpha.SetItem(itemIcon.item);
        }
        
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.state == PlayerState.State.Inventory)
            rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.state == PlayerState.State.Inventory)
        {
            if (InventoryController.Instance.SelectedItemPanel != null)
            {
                InventoryController.Instance.SelectedItemPanel.InsertItem(itemIcon);
            }
            Destroy(rectTransform.gameObject);
            rectTransform = null;
        }
            
    }
    public void DestroyItemAlpha()
    {
        if(rectTransform != null)
        {
            Destroy(rectTransform.gameObject);
            rectTransform = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO 더블클릭 시 여러 기능 추가
    }

    
}
