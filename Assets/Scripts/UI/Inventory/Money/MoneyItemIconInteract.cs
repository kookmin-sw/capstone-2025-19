using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MoneyItemIcon))]
public class MoneyItemIconInteract : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    MoneyItemIcon itemIcon;
    RectTransform rectTransform;
    [SerializeField] GameObject itemIconAlphaPrefab;
    void Awake()
    {
        itemIcon = GetComponent<MoneyItemIcon>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
        {
            rectTransform = Instantiate(itemIconAlphaPrefab, InventoryController.Instance.itemIconParent).GetComponent<RectTransform>();
            rectTransform.transform.position = transform.position;
            
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
            if (InventoryController.Instance.SelectedItemPanel != null && InventoryController.Instance.SelectedItemPanel is InventoryPanel)
            {
                InventoryController.Instance.GetMoney(itemIcon);
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
