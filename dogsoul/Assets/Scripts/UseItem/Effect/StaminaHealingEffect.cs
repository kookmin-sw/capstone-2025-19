using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/StaminaHealingEffect")]
public class StaminaHealingEffect : ItemEffect
{
    [SerializeField] public float staminaHealingValue;
    [SerializeField] public float time;
    public override bool Effect()
    {
        PlayerStatusController.Instance.RecoveryStaminaBuff(staminaHealingValue, time, this);
        return true;
    }

    public override void RemoveEffect()
    {
        PlayerStatusController.Instance.ResetStaminaBuff(staminaHealingValue);
    }
}
