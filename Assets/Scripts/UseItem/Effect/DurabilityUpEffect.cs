using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/DurabilityUpEffect")]
public class DurabilityUpEffect : ItemEffect
{
    [SerializeField] private float duabilityValue;
    public override bool Effect()
    {
        return InventoryController.Instance.WeaponRepair(duabilityValue);
    }

    public override void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }

    
}
