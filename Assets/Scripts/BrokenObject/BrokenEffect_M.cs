using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect_M : BrokenEffect
{

    PhotonView photonView;

    


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    protected override void ActiveTrigger()
    {
        photonView.RPC("ActiveTriggerRPC", RpcTarget.Others);
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
    }

    protected override IEnumerator removeTimer()
    {
        yield return new WaitForSeconds(removeTime);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void ActiveTriggerRPC()
    {
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
    }
    
}
