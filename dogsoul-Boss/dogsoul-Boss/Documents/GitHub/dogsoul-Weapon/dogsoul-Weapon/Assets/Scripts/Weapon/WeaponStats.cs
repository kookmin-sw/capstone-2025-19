using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponStats : ScriptableObject
{
    [Header("Weapon Visual")]
    public GameObject modelPrefab;

    [Header("Weapon Stats")]
    public float attack = 10f;
    public float speed = 10f;
    public float range = 10f;

    [Header("Weapon Animations")]
    public AnimatorOverrideController overrideController;
    public List<string> AttackAnimationNames;
}