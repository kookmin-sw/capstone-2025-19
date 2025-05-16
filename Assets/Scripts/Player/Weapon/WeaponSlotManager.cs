using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    protected WeaponHolderSlot leftHandSlot;
    [SerializeField] protected WeaponHolderSlot rightHandSlot;

    protected DamageCollider leftHandDamageCollider;
    protected DamageCollider rightHandDamageCollider;

    protected ProjectileShooter leftShooter;
    protected ProjectileShooter rightShooter;

    protected virtual void Awake()
    {
        /*WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach(WeaponHolderSlot weaponSlot in weaponHolderSlots)
        {
            if(weaponSlot.isLeftHandSlot)
            {
                leftHandSlot = weaponSlot;
            }
            else if (weaponSlot.isRightHandSlot)
            {
                rightHandSlot = weaponSlot;
            }
        }*/
    }

    public void LoadWeaponOnSlot(WeaponStats weaponItem, bool isLeft)
    {
        //if (weaponItem == null) return;
        if(isLeft)
        {
            leftHandSlot.LoadWeaponModel(weaponItem);
            LoadLeftWeaponDamageCollider();
            LoadLeftProejctileShooter();
        }
        else
        {
            rightHandSlot.LoadWeaponModel(weaponItem);
            LoadRightWeaponDamageCollider();
            LoadRightProejctileShooter();
        }
    }


    #region Handle Weapon's Damage Collider

    protected void LoadLeftWeaponDamageCollider()
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    protected void LoadLeftProejctileShooter()
    {
        leftShooter = rightHandSlot.currentWeaponModel.GetComponentInChildren<ProjectileShooter>();
    }

    protected void LoadRightWeaponDamageCollider()
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    protected void LoadRightProejctileShooter()
    {
        rightShooter = rightHandSlot.currentWeaponModel.GetComponentInChildren<ProjectileShooter>();
    }

    public void OpenRightDamageCollider()
    {
        if (rightHandDamageCollider != null) rightHandDamageCollider.EnableDamageCollider();
        else if (rightShooter != null) rightShooter.Shoot();
    }

    public void OpenLeftDamageCollider()
    {
        if (leftHandDamageCollider != null) leftHandDamageCollider.EnableDamageCollider();
        else if (leftShooter != null) leftShooter.Shoot();
    }

    public virtual void CloseRightDamageCollider()
    {
        rightHandDamageCollider.UnableDamageCollider();
    }

    public void CloseLeftDamageCollider()
    {
        leftHandDamageCollider.UnableDamageCollider();
    }

    #endregion
}
