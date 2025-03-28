using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : MonoBehaviour
{
    [SerializeField] ItemData itemData;




    // Start is called before the first frame update
    void Start()
    {
        Item item = new Item(itemData, itemData.maxQuantity, itemData.maxItemDurability);
        GameObject go = InventoryController.Instance.GetCreateDropItem(item);    
        go.transform.position = transform.position;

        //Instantiate(Resources.Load<GameObject>("Prefabs/Player/DemoPlayer_Village")).transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
