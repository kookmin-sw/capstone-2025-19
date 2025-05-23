using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DropItem_M : DropItem
{
    PhotonView photonView;

    protected override void Init()
    {
        photonView = GetComponent<PhotonView>();
        base.Init();
    }

    public override void SetItem(Item item)
    {
        string itemData = ChangeData(item);
        //DebugText.Instance.Debug($"set item {itemData}");
        if (PhotonNetwork.IsMasterClient)//마스터 클라이언트일 경우
        {
            UpdateItem(item);
            //DebugText.Instance.Debug($"Set Item {itemData} masterClient");
            photonView.RPC("UpdateItemPhoton", RpcTarget.OthersBuffered, itemData);
        }
        else// 기본 클라이언트일 경우
        {
            //DebugText.Instance.Debug($"Set item {itemData} client");
            photonView.RPC("RequestUpdateItemPhoton", RpcTarget.MasterClient, itemData);//마스터 클라이언트에게 변경 요청
        }
    }
    public override void RemoveDropItem()
    {
        itemIcon = null;
        if (!photonView.IsMine)
        {
            photonView.RPC("RequestDeactivateDropItem", photonView.Owner, photonView.ViewID);
        }
        else
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, photonView.ViewID);

        }
    }

    public override void ActivateDropItem(Transform position)
    {
        this.gameObject.transform.position = position.transform.position;
        if (!photonView.IsMine)
        {
            photonView.RPC("RequestActivateDropItemphoton", photonView.Owner, photonView.ViewID);
        }
        else
        {
            photonView.RPC("ActiavateDropItemPhoton", RpcTarget.AllBuffered, photonView.ViewID);

        }
    }

    [PunRPC]
    public void RequestUpdateItemPhoton(string itemData)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DebugText.Instance.Debug($"itemData requestUpdateItemPhoton {itemData}");
            //string itemData = ChangeData(item); // 데이터를 문자열로 변환
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, itemData);
        }
    }

    [PunRPC]
    private void UpdateItemPhoton(string itemData)
    {

        string[] data = itemData.Split('|'); // 문자열을 분할하여 값 추출
        this.itemData = ItemDatabase.Instance.GetItemDataByName(data[0]);
        quantity = int.Parse(data[1]);
        durability = float.Parse(data[2]);
        DebugText.Instance.Debug($"qunantity {quantity} durability {durability}");
        Debug.Log($"UpdateItemPhoton {quantity} {durability}");
    }

    [PunRPC]
    void DeactivateDropItem(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            DebugText.Instance.Debug("DeactivateDropItem");
            InventoryController.Instance.dropItemList.Remove(targetView.GetComponent<DropItem>());
            if (targetView.GetComponent<DropItem>().itemIcon != null)
            {
                targetView.GetComponent<DropItem>().itemIcon.GetComponent<ItemIcon>().RemoveItemIcon();
            }
            targetView.GetComponent<DropItem>().itemIcon = null;
            targetView.gameObject.SetActive(false);
        }
    }
    [PunRPC]
    void RequestDeactivateDropItem(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine) //  소유자만 실행
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, viewID);
        }
    }

    [PunRPC]
    void ActiavateDropItemPhoton(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.gameObject.SetActive(true);
        }
    }
    [PunRPC]
    void RequestActivateDropItemphoton(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine)
        {
            photonView.RPC("ActiavateDropItemPhoton", RpcTarget.AllBuffered, photonView.ViewID);
        }
    }

}
