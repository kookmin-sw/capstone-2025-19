using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemIconAlpha : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    // Start is called before the first frame update
    public void SetItem(Item item)
    {
        itemImage.sprite = item.itemData.itemIcon;
        itemName.text = item.itemData.name;
        itemName.raycastTarget = false;
    }
}
