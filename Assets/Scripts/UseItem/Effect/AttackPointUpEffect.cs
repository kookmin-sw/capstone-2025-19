using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointUpEffect : ItemEffect
{
    [SerializeField] public float powerUpValue;
    [SerializeField] public float time;
    public override bool Effect()
    {
        //throw new System.NotImplementedException();
        PlayerStatusController.Instance.AttackPointUpBuff(powerUpValue, time, this);
        return true;
    }

    public override void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }


}
