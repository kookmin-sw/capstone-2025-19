using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemInteract : InteractGo
{
    public override void CloseInteract()
    {
        //NOthing
    }

    public override void InteractObject()
    {
        DropItem item = InventoryController.Instance.dropItemList[0];
        InventoryController.Instance.PickupDropItem(item);
    }

 
}
