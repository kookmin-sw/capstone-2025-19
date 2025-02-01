using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemPanel))]
public class ItemPanelInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    ItemPanel itemPanel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemPanel.inventoryController.SelectedItemPanel = itemPanel;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemPanel.inventoryController.SelectedItemPanel = null;
    }

    void Awake()
    {
        itemPanel = GetComponent<ItemPanel>();
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
