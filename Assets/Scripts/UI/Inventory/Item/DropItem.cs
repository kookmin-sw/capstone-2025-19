using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Windows;
using System.Runtime.CompilerServices;


public class DropItem : MonoBehaviour
{
    [SerializeField] Transform itemModelPosition;
    GameObject itemModel;
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
        if(SceneController.Instance == null) { return; }
        Init();
    }

    private void Init()
    {
        photonView = GetComponent<PhotonView>();
        if(this.itemData != null)
        {
            Item item = new Item(this.itemData, this.quantity, this.durability);
            SetItem(item);
        }
    }

    public void SetItem(Item item)
    {
        if(SceneController.Instance.GetCurrentSceneName() == "Dungeon_Multiplay")
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
        else
        {
            UpdateItem(item);
        }
        
        
    }
    private void UpdateItem(Item item)
    {
        this.itemData = item.itemData;
        quantity = item.quantity;
        durability = item.durability;

        if(itemModel != null) { Destroy(itemModel); itemModel = null; }

        itemModel = Instantiate(Resources.Load<GameObject>($"Prefabs/Objects/DropItem/DropItemModel/{item.itemData.name}_DropItem"));
        itemModel.transform.position = itemModelPosition.position;
        itemModel.transform.SetParent(itemModelPosition.transform);
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
        if (SceneController.Instance.GetCurrentSceneName() == "Dungeon_Multiplay")
        {
            if (!photonView.IsMine)
            {
                photonView.RPC("RequestDeactivateDropItem", photonView.Owner, photonView.ViewID);
            }
            else
            {
                photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, photonView.ViewID);

            }
        }
        else
        {
            InventoryController.Instance.dropItemList.Remove(this);
            gameObject.SetActive(false);
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
        if(SceneController.Instance.GetCurrentSceneName() == "Dungeon_Multiplay")
        {
            if (!photonView.IsMine)
            {
                photonView.RPC("RequestActivateDropItemphoton", photonView.Owner, photonView.ViewID);
            }
            else
            {
                photonView.RPC("ActiavateDropItemPhoton", RpcTarget.AllBuffered, photonView.ViewID);

            }
        }
        else
        {
            gameObject.SetActive(true);
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
