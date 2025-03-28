using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory_ : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;
    public WeaponStats rightWeapon;

    void Awake()
    {
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    void Start()
    {
        weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
    }
}