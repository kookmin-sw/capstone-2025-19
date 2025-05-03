using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] List<ItemData> storeItemData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInventoryState(bool input)
    {

        InventoryController.Instance.SetStoreInventory(input);
        if (input) { InventoryController.Instance.storeItemPanel.InsertItem(storeItemData); }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { SetInventoryState(true); }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { SetInventoryState(false); }
    }
}
