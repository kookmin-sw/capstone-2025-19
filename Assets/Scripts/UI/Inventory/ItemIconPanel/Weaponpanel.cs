using System.Collections;
using System.Collections.Generic;
using PlayerCombat;
using Unity.VisualScripting;
using UnityEngine;

public class Weaponpanel : ItemPanel
{
    // Start is called before the first frame update
    ItemIcon weaponItemIcon;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void InsertItem(ItemIcon itemIcon)
    {
        if (itemIcon.item.itemData.itemType_ != ItemData.ItemType.Weapon) { return; }
        if (weaponItemIcon != null)
        {
            ChangeWeapon();
            return; // 당장은 return
        }
        base.InsertItem(itemIcon);
        itemIcon.transform.SetParent(transform);
        weaponItemIcon = itemIcon;
        Debug.Log($"itemIcon {itemIcon.item.itemData}");
        Debug.Log($"itemIcon {itemIcon.item.itemData.weaponStats}");
        //TODO player에게 무기 쥐어주기
        InventoryController.Instance.weaponSlotManager.LoadWeaponOnSlot(weaponItemIcon.item.itemData.weaponStats, false);
        //WeaponSlotManager.Instance.LoadWeaponOnSlot(weaponItemIcon.item.itemData.weaponStats, false);
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        //TODO 무기 지우기
    }
    private void ChangeWeapon()
    {
        //TODO ChangeWeapon
    }

    public WeaponStats GetWeapon()
    {
        if (weaponItemIcon != null)
        {
            return weaponItemIcon.item.itemData.weaponStats;
        }
        return null;
    }

    public void SetWeapon()
    {
        if(weaponItemIcon != null)
        {
            InventoryController.Instance.weaponSlotManager.LoadWeaponOnSlot(weaponItemIcon.item.itemData.weaponStats, false);
            //WeaponSlotManager.Instance.LoadWeaponOnSlot(weaponItemIcon.item.itemData.weaponStats, false);
        }
    }
}
