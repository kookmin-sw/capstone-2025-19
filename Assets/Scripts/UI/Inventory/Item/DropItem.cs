using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Windows;
using System.Runtime.CompilerServices;


public class DropItem : MonoBehaviour
{
    public GameObject itemIcon = null;
    PhotonView photonView;
    public int quantity = 1;
    public float durability = 1f;
    public ItemData itemData;

    public void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void SetItem(Item item)
    {
        string itemData = ChangeData(item);
        DebugText.Instance.Debug($"set item {itemData}");
        if (PhotonNetwork.IsMasterClient)//������ Ŭ���̾�Ʈ�� ���
        {
            UpdateItem(item);
            DebugText.Instance.Debug($"Set Item {itemData} masterClient");
            photonView.RPC("UpdateItemPhoton", RpcTarget.OthersBuffered, itemData);
        }
        else// �⺻ Ŭ���̾�Ʈ�� ���
        {
            DebugText.Instance.Debug($"Set item {itemData} client");
            photonView.RPC("RequestUpdateItemPhoton", RpcTarget.MasterClient, itemData);//������ Ŭ���̾�Ʈ���� ���� ��û
        }
        
    }
    private void UpdateItem(Item item)
    {
        this.itemData = item.itemData;
        quantity = item.quantity;
        durability = item.durability;
    }
    [PunRPC]
    public void RequestUpdateItemPhoton(string itemData)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DebugText.Instance.Debug($"itemData requestUpdateItemPhoton {itemData}");
            //string itemData = ChangeData(item); // �����͸� ���ڿ��� ��ȯ
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, itemData);
        }
    }

    [PunRPC]
    private void UpdateItemPhoton(string itemData)
    {
        
        string[] data = itemData.Split('|'); // ���ڿ��� �����Ͽ� �� ����
        this.itemData = ItemDatabase.Instance.GetItemDataByName(data[0]);
        quantity = int.Parse(data[1]);
        durability = float.Parse(data[2]);
        DebugText.Instance.Debug($"qunantity {quantity} durability {durability}");
        Debug.Log($"UpdateItemPhoton {quantity} {durability}");
    }



    /*public void SetItem(Item item)
    {
        string itemData = ChangeData(item);
        if (!photonView.IsMine)
        {
            photonView.RPC("")
        }
    }*/

    public void RemoveDropItem()
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

    
    [PunRPC]
    void DeactivateDropItem(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            DebugText.Instance.Debug("DeactivateDropItem");
            InventoryController.Instance.dropItemList.Remove(targetView.GetComponent<DropItem>());
            if(targetView.GetComponent<DropItem>().itemIcon != null)
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
        if (targetView != null && targetView.IsMine) //  �����ڸ� ����
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, viewID);
        }
    }

    public void ActivateDropItem(Transform position)
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
    void ActiavateDropItemPhoton(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if(targetView != null)
        {
            targetView.gameObject.SetActive(true);
        }
    }
    [PunRPC]
    void RequestActivateDropItemphoton(int viewID)
    {
        PhotonView targetView = PhotonView.Find (viewID);
        if(targetView != null && targetView.IsMine)
        {
            photonView.RPC("ActiavateDropItemPhoton", RpcTarget.AllBuffered, photonView.ViewID);
        }
    }
    /*[PunRPC]
    private void UpdateItemPhoton_(Item item)
    {
        quantity = item.quantity;
        durability = item.durability;
    }
    [PunRPC]
    private void RequestUpdateItemPhoton_(Item item)
    {
        if (PhotonNetwork.IsMasterClient) // �����Ϳ��� ����
        {
            UpdateItemPhoton_(item);
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, item);
        }
    }*/
    

    

    private string ChangeData(Item item)
    {
        return string.Join("|", item.itemData.name, item.quantity, item.durability);
    }

    

}
