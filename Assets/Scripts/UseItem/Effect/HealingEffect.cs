using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/HealingEffect")]
public class HealingEffect : ItemEffect
{
    public float healAmount;

    public override void Effect()
    {
        Debug.Log($"Healing {healAmount} HP.");
        //TODO PlayerStatusController ฐüทร 
    }

    public string EffectInfo()
    {
        return $"Player HP healing {healAmount}";
    }
}
