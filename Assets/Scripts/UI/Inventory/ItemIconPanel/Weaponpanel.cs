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
        if (itemIcon.item.itemData.itemType != ItemData.ItemType.Weapon) { return; }
        if (weaponItemIcon != null)
        {
            ChangeWeapon();
            //return; 
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
        weaponItemIcon = null;
        //TODO 무기 지우기
        InventoryController.Instance.weaponSlotManager.LoadWeaponOnSlot(null, false);
    }
    private void ChangeWeapon()
    {
        //TODO ChangeWeapon
        InventoryController.Instance.inventoryPanel.InsertItem(weaponItemIcon);
        weaponItemIcon = null;
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

    public ItemIcon GetWeaponItemIcon()
    {
        return weaponItemIcon;
    }

    public override void RemoveItem(ItemIcon itemIcon)
    {
        weaponItemIcon = null;
        InventoryController.Instance.weaponSlotManager.LoadWeaponOnSlot(null, false);
    }


}
