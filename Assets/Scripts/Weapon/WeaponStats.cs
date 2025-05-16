using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class WeaponStats : ScriptableObject
{

    public GameObject weaponPrefab;
    public string weaponType;
    public float damage;
    public bool isRanged;
    public GameObject projectile;
    public ParticleSystem hitEffect;
    public bool isLeft;
    public float staminaUsage;
    public float tenacity;
}