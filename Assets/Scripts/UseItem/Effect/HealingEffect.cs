using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/HealingEffect")]
public class HealingEffect : ItemEffect
{
    public float healAmount;

    public override bool Effect()
    {
        Debug.Log($"Healing {healAmount} HP.");
        //TODO PlayerStatusController ฐüทร 
        PlayerStatusController.Instance.curHp += healAmount;
        return true;
    }

    public string EffectInfo()
    {
        return $"Player HP healing {healAmount}";
    }

    public override void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
