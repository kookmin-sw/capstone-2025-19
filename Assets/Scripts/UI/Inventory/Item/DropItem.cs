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
        if (PhotonNetwork.IsMasterClient)//������ Ŭ���̾�Ʈ�� ���
        {
            UpdateItem(item);
            DebugText.Instance.Debug($"{itemData} masterClient");
            photonView.RPC("UpdateItemPhoton", RpcTarget.OthersBuffered, itemData);
        }
        else// �⺻ Ŭ���̾�Ʈ�� ���
        {
            DebugText.Instance.Debug($"{itemData} client");
            photonView.RPC("RequestUpdateItemPhoton", RpcTarget.MasterClient, itemData);//������ Ŭ���̾�Ʈ���� ���� ��û
        }
        
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
            photonView.RPC("RequestDestroyDropItem", RpcTarget.MasterClient, photonView.ViewID);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
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
    [PunRPC]
    void RequestDestroyDropItem(int viewID)
    {
        if (PhotonNetwork.IsMasterClient) // �����͸� ���� ����
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView != null)
            {
                PhotonNetwork.Destroy(targetView.gameObject);
            }
        }
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
        Debug.Log(data[0]);
        Debug.Log(this.itemData);
    }

    private void UpdateItem(Item item)
    {
        this.itemData = item.itemData;
        quantity = item.quantity;
        durability = item.durability;
    }

    private string ChangeData(Item item)
    {
        return string.Join("|", item.itemData.name, item.quantity, item.durability);
    }

}
