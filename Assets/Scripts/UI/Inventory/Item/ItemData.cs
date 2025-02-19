using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Potion,
        Weapon,
        Armor,
        Backpack,

    }
    public ItemType itemType_;

    public Sprite itemIcon;
    public string itemType;

    public float size = 1;
    public float Weight = 1;

    public bool isUse;
    public float containerValue;

    public int maxQuantity = 1;
    public int price;
    public int maxItemDurability = 1;
}
