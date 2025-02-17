using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemIconAlpha : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameText;
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
        itemImage.sprite = item.itemData.itemIcon;
        itemNameText.text = item.itemData.name;
        itemNameText.raycastTarget = false;
    }
}
