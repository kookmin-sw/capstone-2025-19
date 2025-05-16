using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public AnimationHandler animationHandler;
    public Transform parentOverride;
    public bool isLeftHandSlot;
    public bool isRightHandSlot;

    public GameObject currentWeaponModel;

    private Animator animator;

    PhotonView photonView;

    protected virtual void Awake()
    {
        animator = GetComponentInParent<Animator>();
        photonView = GetComponent<PhotonView>();
        animationHandler = GetComponentInParent<AnimationHandler>();
    }

    public void UnloadWeapon()
    {
        if(currentWeaponModel != null)
        {
            currentWeaponModel.SetActive(false);
        }
    }

    public virtual void UnloadWeaponAndDestroy()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }
    public virtual void LoadWeaponModel(WeaponStats weaponStats)
    {
        //Internal_LoadWeapon(weaponStats);
        UnloadWeaponAndDestroy();

        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }
        GameObject weapon = null;

        weapon = Instantiate(weaponStats.weaponPrefab) as GameObject;
        if (!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            weaponCollider.hitEffect = weaponStats.hitEffect;
            weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        }

        if (weapon != null)
        {
            if (parentOverride != null)
            {
                weapon.transform.parent = parentOverride;
            }
            else
            {
                weapon.transform.parent = transform;
            }

            if (animationHandler != null)
            {
                print("animator updated");
                animationHandler.UpdateOverride(weaponStats.weaponType);
            }
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;
        }

        currentWeaponModel = weapon;


    }

    protected virtual void Internal_LoadWeapon(WeaponStats weaponStats)
    {
        UnloadWeaponAndDestroy();
        if (weaponStats == null)
        {
            UnloadWeapon();
            return;
        }

        var weapon = Instantiate(weaponStats.weaponPrefab);

        weapon.transform.SetParent(parentOverride != null ? parentOverride : transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;
        currentWeaponModel = weapon;


        if (!weaponStats.isRanged)
        {
            DamageCollider weaponCollider = weapon.GetComponentInChildren<DamageCollider>();
            weaponCollider.damage = weaponStats.damage;
            weaponCollider.tenacity = weaponStats.tenacity;
            weaponCollider.hitEffect = weaponStats.hitEffect;
            weaponCollider.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        }

        // 여기서 Override Controller 적용
        if (animationHandler != null)
            animationHandler.UpdateOverride(weaponStats.weaponType);
    }

    
}
