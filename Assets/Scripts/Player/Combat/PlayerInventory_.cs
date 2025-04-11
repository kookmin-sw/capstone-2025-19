using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory_ : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;
    public WeaponStats weapon;
    public bool isRanged;
    public float staminaUsage;

    void Awake()
    {
        weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    void Start()
    {
        weaponSlotManager.LoadWeaponOnSlot(weapon, weapon.isLeft);
        isRanged = weapon.isRanged;
        staminaUsage = weapon.staminaUsage;
    }
}