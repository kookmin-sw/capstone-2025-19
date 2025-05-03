using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MoneyDropItem_M : MoneyDropItem
{
    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void Init()
    {
        photonView = GetComponent<PhotonView>();
        base.Init();
    }

    public override void SetValue(int moneyValue)
    {

        if (PhotonNetwork.IsMasterClient)//������ Ŭ���̾�Ʈ�� ���
        {
            UpdateValue(moneyValue);
            DebugText.Instance.Debug($"Set Item {moneyValue} masterClient");
            photonView.RPC("UpdateItemPhoton", RpcTarget.OthersBuffered, moneyValue);
        }
        else// �⺻ Ŭ���̾�Ʈ�� ���
        {
            DebugText.Instance.Debug($"Set item {moneyValue} client");
            photonView.RPC("RequestUpdateItemPhoton", RpcTarget.MasterClient, moneyValue);//������ Ŭ���̾�Ʈ���� ���� ��û
        }

    }

    public override void RemoveDropItem()
    {
        MoneyItemIcon = null;

        if (!photonView.IsMine)
        {
            photonView.RPC("RequestDeactivateDropItem", photonView.Owner, photonView.ViewID);
        }
        else
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, photonView.ViewID);

        }

    }

    [PunRPC]
    public void RequestUpdateItemPhoton(int moneyValue)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DebugText.Instance.Debug($"itemData requestUpdateItemPhoton {moneyValue}");
            //string itemData = ChangeData(item); // �����͸� ���ڿ��� ��ȯ
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, moneyValue);
        }
    }
    [PunRPC]
    private void UpdateItemPhoton(int moneyValue)
    {

        this.moneyValue = moneyValue;
    }

    [PunRPC]
    void DeactivateDropItem(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            DebugText.Instance.Debug("DeactivateDropItem");
            //InventoryController.Instance.dropItemList.Remove(targetView.GetComponent<DropItem>());
            if (targetView.GetComponent<MoneyDropItem>().MoneyItemIcon != null)
            {
                targetView.GetComponent<MoneyDropItem>().MoneyItemIcon.GetComponent<MoneyItemIcon>().RemoveItemIcon();
            }
            targetView.GetComponent<MoneyDropItem>().MoneyItemIcon = null;
            targetView.gameObject.SetActive(false);
        }
    }
    [PunRPC]
    void RequestDeactivateDropItem(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null && targetView.IsMine) //  �����ڸ� ����
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, viewID);
        }
    }

}
