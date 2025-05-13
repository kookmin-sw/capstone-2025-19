using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MoneyPanel))]
public class MoneyPanelInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] GameObject itemIconAlpha;
    RectTransform rectTransform;


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
        {
            if (InventoryController.Instance.money > 0)
            {
                rectTransform = Instantiate(itemIconAlpha, InventoryController.Instance.itemIconParent).GetComponent<RectTransform>();
                rectTransform.transform.position = transform.position;
            }

        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory && InventoryController.Instance.money > 0)
            rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory && InventoryController.Instance.money > 0)
        {
            if (InventoryController.Instance.SelectedItemPanel != null && InventoryController.Instance.SelectedItemPanel is DropItemPanel)
            {
                InventoryController.Instance.DropMoney();
            }
            Destroy(rectTransform.gameObject);
            rectTransform = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
