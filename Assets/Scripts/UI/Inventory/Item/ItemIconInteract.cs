using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(ItemIcon))]
public class ItemIconInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler
{
    ItemIcon itemIcon;
    RectTransform rectTransform;
    [SerializeField] GameObject itemIconAlphaPrefab;
    GameObject itemIconAlpha;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f; // ����Ŭ�� �ð� ���� (��)


    void Awake()
    {
        itemIcon = GetComponent<ItemIcon>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
        {
            rectTransform = Instantiate(itemIconAlphaPrefab, InventoryController.Instance.itemIconParent).GetComponent<RectTransform>();
            rectTransform.transform.position = transform.position;
            ItemIconAlpha alpha = rectTransform.GetComponent<ItemIconAlpha>();
            alpha.SetItem(itemIcon.item);
        }
        
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
            rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
        {
            if (InventoryController.Instance.SelectedItemPanel != null)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    //TODO ������ �й�
                    Debug.Log("item test");
                    InventoryController.Instance.distributionPanel.SetItem(InventoryController.Instance.SelectedItemPanel, itemIcon);
                }
                else
                {

                    InventoryController.Instance.SelectedItemPanel.InsertItem(itemIcon);
                }
            }
            else if( InventoryController.Instance.purchasePanelBool)
            {
                //TODO purchase �гο� ���� ���

                InventoryController.Instance.purchasePanel.InsertPlayerItem(itemIcon);
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
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            Debug.Log("UI ����Ŭ�� ����!");
            OnDoubleClick();
        }
        lastClickTime = Time.time;
    }

    private void OnDoubleClick()
    {
        itemIcon.item.UseItem();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ItemIcon enter");
    }
}
