using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{
    public GameObject modelPrefab;
    public bool isUnarmed;

    [Header("One Handed Attack Animations")]
    public string OH_Light_Attack_1;
    public string OH_Light_Attack_2;
    public string OH_Heavy_Attack_1;


    public int attackPower;
    public int speed;
    public AnimatorOverrideController controller;


}
