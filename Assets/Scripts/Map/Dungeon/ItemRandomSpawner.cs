using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRandomSpawner : MonoBehaviour
{
    [SerializeField] SerializableArray<ItemData, float> spawnArray;
    [SerializeField] float spawnRange = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void SpawnItem()
    {
        foreach(var data in spawnArray)
        {
            if (data.Value > Random.Range(0, 1))
            {
                //TODO CreateDropItem
                ItemData itemData = data.Key;
                Item item = new Item(itemData, itemData.maxQuantity, itemData.maxItemDurability);
                GameObject dropItemGo = InventoryController.Instance.GetCreateDropItem(item);
                Vector2 randomInCircle = Random.insideUnitCircle * spawnRange;
                Vector3 spawnPosition = new Vector3(transform.position.x + randomInCircle.x, transform.position.y, transform.position.z + randomInCircle.y);
                dropItemGo.transform.position = spawnPosition;
                //하나만 생성 할 지 여러개 생성 할 지 고민
            }
        }
    }


}
