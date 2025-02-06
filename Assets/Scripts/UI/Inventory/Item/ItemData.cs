using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{

    

    public Sprite itemIcon;
    public string itemType;

    public float size;
    public float Weight;

    public bool isUse;
    public bool isContainer;

    public int maxQuantity;
    public int price;
    public int maxItemDurability;
}
