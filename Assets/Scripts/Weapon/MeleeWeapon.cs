using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public abstract class MeleeWeapon : WeaponSystem
{
    [SerializeField] Collider weaponCollider;
    abstract public override void Attack();
    

    
}
