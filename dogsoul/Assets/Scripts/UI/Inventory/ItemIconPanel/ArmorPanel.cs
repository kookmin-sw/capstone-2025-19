using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPanel : ItemPanel
{
    // Start is called before the first frame update
    ItemIcon armorItemIcon;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void InsertItem(ItemIcon itemIcon)
    {
        if (itemIcon.item.itemData.itemType != ItemData.ItemType.Armor) { return; }
        if(armorItemIcon != null) { ChangeArmor();
            return; //¥Á¿Â¿∫ return
        }
        base.InsertItem(itemIcon);
        itemIcon.transform.SetParent(transform);
        //TODO playerø°∞‘ ∞©ø  ¿‘«Ù¡÷±‚
    }
    public override void TakeOutItem(ItemPanel itemPanel, ItemIcon itemIcon)
    {
        base.TakeOutItem(itemPanel, itemIcon);
        //TODO ∞©ø  π˛±‚±‚
    }
    private void ChangeArmor()
    {
        //TODO ChangeArmor

    }
}
