using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static UnityEditor.Progress;

public class ItemIcon : MonoBehaviour
{
    public Transform itemIconParent;
    public InventoryController inventoryController;
    public Item item;
    public ItemPanel itemPanel;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemNameText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(Item item)
    {
        this.item = item;
        itemIconImage.sprite = item.itemData.itemIcon;
        itemNameText.text = item.itemData.name;
        
    }
}
