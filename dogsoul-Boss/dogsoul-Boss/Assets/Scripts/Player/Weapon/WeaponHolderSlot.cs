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
    public void LoadWeaponModel(WeaponStats weaponStats)
    {
        UnloadWeaponAndDestroy();

        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }

        GameObject weapon = Instantiate(weaponStats.weaponPrefab) as GameObject;

        if(!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();    
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        }
        
        if (transform.root.tag == "Player") animator.runtimeAnimatorController = weaponStats.weaponOverride;
        
        if(weapon != null)
        {
            if(parentOverride != null)
            {
                weapon.transform.parent = parentOverride;
            }
            else
            {
                weapon.transform.parent = transform;
            }

            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
        }

        currentWeaponModel = weapon;
    }
}
