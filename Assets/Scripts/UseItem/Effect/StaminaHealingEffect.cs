using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaHealingEffect : ItemEffect
{
    [SerializeField] public float staminaHealingValue;
    [SerializeField] public float time;
    public override void Effect()
    {
        PlayerStatusController.Instance.RecoveryStaminaBuff(staminaHealingValue, time);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
