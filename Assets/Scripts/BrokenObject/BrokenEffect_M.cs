using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BrokenEffect_M : BrokenEffect
{
    [SerializeField] InteractGo interactGo;
    PhotonView photonView;

    


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    protected override void ActiveTrigger()
    {
        photonView.RPC("ActiveTriggerRPC", RpcTarget.OthersBuffered);
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
        isBroken = true;
        if(interactGo != null) { InventoryController.Instance.player.interactGoList.Remove(interactGo); }
    }

    protected override IEnumerator removeTimer()
    {
        yield return new WaitForSeconds(removeTime);
        Debug.Log($"Destory {gameObject.name}");
        Destroy(gameObject);
    }

    [PunRPC]
    private void ActiveTriggerRPC()
    {
        Debug.Log("RPC receive");
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
        isBroken = true;
        if (interactGo != null) { InventoryController.Instance.player.interactGoList.Remove(interactGo); }
        StartCoroutine(removeTimer());
    }

    
    
}
