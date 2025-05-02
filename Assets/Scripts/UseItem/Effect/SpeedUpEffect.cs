using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffect/SpeedUpEffect")]
public class SpeedUpEffect : ItemEffect
{
    [SerializeField] private float speedUpValue;
    [SerializeField] private float timeValue;
    public override bool Effect()
    {
        PlayerStatusController.Instance.SpeedUpBuff(speedUpValue, timeValue, this);
        return true;
    }

    public override void RemoveEffect()
    {
        PlayerStatusController.Instance.ResetSpeed(speedUpValue);
    }

    
}
