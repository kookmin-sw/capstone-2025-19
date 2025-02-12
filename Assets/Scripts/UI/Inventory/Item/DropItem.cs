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

    public void RemoveDropItem_()
    {
        itemIcon = null;
        if (!photonView.IsMine)
        {
            photonView.RPC("RequestDestroyDropItem", photonView.Owner, photonView.ViewID);
        }
        else
        {
            photonView.RPC("RemoveItemIcon", RpcTarget.AllBuffered, photonView.ViewID);
            
        }
        
    }

    private IEnumerator SetRemoveDropItem()
    {
        Debug.Log("test1");
        yield return new WaitForSeconds(0.1f);
        Debug.Log("test2");
        RemoveDropItem_();
    }

    public void RemoveDropItem()
    {
        itemIcon = null;
        StartCoroutine(SetRemoveDropItem());
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
        /*if (PhotonNetwork.IsMasterClient) // �����͸� ���� ����
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView != null)
            {
                if(!targetView.IsMine)
                {
                    Debug.Log("owner test1");
                    targetView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }

                Debug.Log($"owner test2 {targetView.IsMine}");
                PhotonNetwork.Destroy(targetView.gameObject);
            }
        }*/
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            if (targetView.IsMine)
            {
                //PhotonNetwork.Destroy(targetView.gameObject);
                //targetView.RPC("RemoveItemIcon", RpcTarget.AllBuffered, viewID);
                PhotonNetwork.Destroy(gameObject);
            }

            Debug.Log($"owner test2 {targetView.IsMine}");
            
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

    }
    [PunRPC]
    private void RemoveItemIcon(int viewID)
    {
        Debug.Log("7");
        DebugText.Instance.Debug($"RemoveItemIcon {itemIcon}");
        if(itemIcon != null)
        {
            Debug.Log("8");
            InventoryController.Instance.RemoveItemIcon(this);
        }
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

    private IEnumerator DestroyAfterUIUpdate()
    {
        Debug.Log("9");
        yield return new WaitForSeconds(0.5f); // UI ������Ʈ�� ���� ��� ���
        PhotonNetwork.Destroy(gameObject);
        Debug.Log("10");
    }

}
