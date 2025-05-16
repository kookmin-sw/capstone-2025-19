using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDropItem : MonoBehaviour
{
    [SerializeField] List<ItemData> itemDataList;
    // Start is called before the first frame update
    void Start()
    {
        foreach (ItemData itemData in itemDataList) 
        {
            GameObject go = Instantiate(Resources.Load<GameObject>($"Prefabs/Objects/DropItem/DropItem"));
            DropItem dropItem = go.GetComponent<DropItem>();
            Item item = new Item(itemData, itemData.maxQuantity, itemData.maxItemDurability);
            dropItem.SetItem(item);
            go.transform.position = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
