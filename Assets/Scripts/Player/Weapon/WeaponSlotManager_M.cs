using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager_M : WeaponSlotManager
{
    PhotonView photonView;
    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }
    public override void CloseRightDamageCollider()
    {
        //if(photonView.IsMine)
            base.CloseRightDamageCollider();
    }
}
