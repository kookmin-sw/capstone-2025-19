using System.Collections;
using System.Collections.Generic;
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
            return; // ������ return
        }
        base.InsertItem(itemIcon);
        itemIcon.transform.SetParent(transform);
        //TODO player���� ���� ����ֱ�
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        //TODO ���� �����
    }
    private void ChangeWeapon()
    {
        //TODO ChangeWeapon
    }
}
