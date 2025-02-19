using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : Singleton<ItemDatabase>
{
    private Dictionary<string, ItemData> itemDataDict = new Dictionary<string, ItemData>();
    private void LoadAllItemData()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("ItemData");

        foreach (ItemData data in allItems)
        {
            if (!itemDataDict.ContainsKey(data.name))
            {
                itemDataDict.Add(data.name, data);
            }
        }
    }

    public ItemData GetItemDataByName(string name)
    {
        if(itemDataDict.TryGetValue(name, out ItemData data))
        {
            return data;
        }
        return null;
    }

    public void Start()
    {
        LoadAllItemData();
    }
}
