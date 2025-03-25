using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public Transform parentOverride;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if(currentWeaponModel != null)
        {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }
    public void LoadWeaponModel(WeaponStats weaponItem, Animator animator)
    {
        UnloadWeaponAndDestroy();

        if (weaponItem == null)
        {
            UnloadWeapon();
            return;
        }

        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        AnimatorOverrideController weaponOverride = weaponItem.weaponOverride;
        print(weaponOverride.name);
        if (weaponOverride != null)
        {
            animator.runtimeAnimatorController = weaponOverride;
        }
        else if (overrideController != null)
        {
            animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }

        GameObject model = Instantiate(weaponItem.weaponPrefab) as GameObject;
        if(model != null)
        {
            if(parentOverride != null)
            {
                model.transform.parent = parentOverride;
            }
            else
            {
                model.transform.parent = transform;
            }

            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }

        currentWeaponModel = model;
        DamageCollider Sibal = currentWeaponModel.GetComponentInChildren<DamageCollider>();
        print(Sibal);
    }
}
