using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    WeaponHolderSlot leftHandSlot;
    WeaponHolderSlot rightHandSlot;

    DamageCollider leftHandDamageCollider;
    DamageCollider rightHandDamageCollider;

    ProjectileShooter leftShooter;
    ProjectileShooter rightShooter;

    void Awake()
    {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
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
        }
    }

    public void LoadWeaponOnSlot(WeaponStats weaponItem, bool isLeft)
    {
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

    private void LoadLeftWeaponDamageCollider()
    {
        leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadLeftProejctileShooter()
    {
        leftShooter = rightHandSlot.currentWeaponModel.GetComponentInChildren<ProjectileShooter>();
    }

    private void LoadRightWeaponDamageCollider()
    {
        rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
    }

    private void LoadRightProejctileShooter()
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

    public void CloseRightDamageCollider()
    {
        rightHandDamageCollider.UnableDamageCollider();
    }

    public void CloseLeftDamageCollider()
    {
        leftHandDamageCollider.UnableDamageCollider();
    }

    #endregion
}
