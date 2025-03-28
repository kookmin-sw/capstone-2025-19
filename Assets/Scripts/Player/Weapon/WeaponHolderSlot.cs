using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public Transform parentOverride;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public GameObject currentWeaponModel;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

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
    public void LoadWeaponModel(WeaponStats weaponItem)
    {
        UnloadWeaponAndDestroy();

        if (weaponItem == null)
        {
            UnloadWeapon();
            return;
        }


        GameObject model = Instantiate(weaponItem.weaponPrefab) as GameObject;
        DamageCollider collider = model.GetComponentInChildren<DamageCollider>();

        AnimatorOverrideController weaponOverride = weaponItem.weaponOverride;
        // check override controller matches original controller = when attacker is player
        if (weaponOverride != null && weaponOverride.runtimeAnimatorController == animator.runtimeAnimatorController)
        {
            animator.runtimeAnimatorController = weaponOverride;
            collider.tag = "PlayerWeapon";
        }
        else
        {
            collider.tag = "EnemyWeapon";
        }
        
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
    }
}
