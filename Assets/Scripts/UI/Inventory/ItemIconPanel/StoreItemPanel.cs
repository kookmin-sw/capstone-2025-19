using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemPanel : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    public void InsertItem(List<ItemData> storeItem)
    {
        if(scrollRect.content.childCount > 0) { return; }
        foreach (ItemData itemData in storeItem)
        {
            GameObject storeItemIcon = Instantiate(Resources.Load<GameObject>($"Prefabs/UI/Inventory/Store_ItemIcon"));
            StoreItemIcon _storeItemIcon = storeItemIcon.GetComponent<StoreItemIcon>();
            _storeItemIcon.SetItem(itemData);
            storeItemIcon.transform.SetParent(scrollRect.content);
        }
    }
}
