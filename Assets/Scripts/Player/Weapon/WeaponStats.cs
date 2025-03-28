using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{
    public GameObject weaponPrefab;
    public AnimatorOverrideController weaponOverride;
    public float damage;
    public bool rightHandOnly;
}