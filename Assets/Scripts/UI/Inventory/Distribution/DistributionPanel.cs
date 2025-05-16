using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DistributionPanel : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] Slider quantitySlider;

    [Header("돈 관련")]
    [SerializeField] Sprite moneyImage;
    
    
    [HideInInspector]
    public ItemIcon distributionItemIcon;
    [HideInInspector]
    public ItemPanel itemPanel;



    
    public void SetItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        if(itemIcon.item.itemData.itemType != ItemData.ItemType.Potion) { return; }
        this.itemPanel = itemPanel;
        distributionItemIcon = itemIcon;
        itemImage.sprite = distributionItemIcon.item.itemData.itemIcon;
        itemName.text = distributionItemIcon.item.itemData.name;
        quantityText.text = $"{distributionItemIcon.item.quantity} / {distributionItemIcon.item.quantity}";
        quantitySlider.maxValue = distributionItemIcon.item.quantity;
        quantitySlider.value = distributionItemIcon.item.quantity;
        gameObject.SetActive(true);
        quantitySlider.onValueChanged.AddListener(UpdateValueText);
    }
    public void SetMoney()
    {
        itemImage.sprite = moneyImage;
        quantityText.text = $"{InventoryController.Instance.money} / {InventoryController.Instance.money}";
        quantitySlider.maxValue = InventoryController.Instance.money;
        quantitySlider.value = InventoryController.Instance.money;
        gameObject.SetActive(true);
        quantitySlider.onValueChanged.AddListener (UpdateValueText);
    }

    public void CancellButton()
    {
        gameObject.SetActive(false);
    }
    public void OKButton()
    {
        if(distributionItemIcon != null) //일반 아이템일 경우
        {
            if (quantitySlider.value < distributionItemIcon.item.quantity)
            {
                Item item = new Item(distributionItemIcon.item.itemData, (int)quantitySlider.value, distributionItemIcon.item.durability);
                ItemIcon itemIcon = InventoryController.Instance.GetCreateItemIcon(item);
                itemPanel.InsertItem(itemIcon);
                distributionItemIcon.item.quantity -= (int)quantitySlider.value;
                distributionItemIcon.SetSlider();
            }
            else
            {
                itemPanel.InsertItem(distributionItemIcon);
            }
            gameObject.SetActive(false);
            distributionItemIcon = null;
        }
        else//Money 일 경우
        {

        }
        
        
    }

    private void UpdateValueText(float value)
    {
        quantityText.text = $"{((int)value).ToString()} / {distributionItemIcon.item.itemData.maxQuantity}";
    }


}
