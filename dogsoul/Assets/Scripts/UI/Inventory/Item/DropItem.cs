using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Windows;
using System.Runtime.CompilerServices;


public class DropItem : MonoBehaviour
{
    [SerializeField] protected Transform itemModelPosition;
    protected GameObject itemModel;
    public GameObject itemIcon = null;
    
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

    protected virtual void Init()
    {
        if(this.itemData != null)
        {
            Item item = new Item(this.itemData, this.quantity, this.durability);
            SetItem(item);
        }
    }

    public virtual void SetItem(Item item)
    {
        UpdateItem(item);
    }
    protected void UpdateItem(Item item)
    {
        this.itemData = item.itemData;
        quantity = item.quantity;
        durability = item.durability;

        if(itemModel != null) { Destroy(itemModel); itemModel = null; }

        itemModel = Instantiate(Resources.Load<GameObject>($"Prefabs/Objects/DropItem/DropItemModel/{item.itemData.name}_DropItem"));
        itemModel.transform.position = itemModelPosition.position;
        itemModel.transform.SetParent(itemModelPosition.transform);
    }
    



    /*public void SetItem(Item item)
    {
        string itemData = ChangeData(item);
        if (!photonView.IsMine)
        {
            photonView.RPC("")
        }
    }*/

    public virtual void RemoveDropItem()
    {
        itemIcon = null;
        InventoryController.Instance.dropItemList.Remove(this);
        gameObject.SetActive(false);

    }

    


    public virtual void ActivateDropItem(Transform position)
    {
        this.gameObject.transform.position = position.transform.position;
        gameObject.SetActive(true);
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
        if (PhotonNetwork.IsMasterClient) // 마스터에서 실행
        {
            UpdateItemPhoton_(item);
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, item);
        }
    }*/
    

    

    protected string ChangeData(Item item)
    {
        return string.Join("|", item.itemData.name, item.quantity, item.durability);
    }

    

}
