using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory_ : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;
    private Animator animator;

    public WeaponStats rightWeapon;

    void Awake()
    {
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
    }
}

