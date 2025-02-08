using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DropItem : MonoBehaviour
{
    PhotonView photonView;
    public Item item;
    public GameObject go { get; set; }

    private Collider _collider;

    private void Init()
    {
        _collider = GetComponent<Collider>();
        if( _collider == null)
        {
            _collider = GetComponentInChildren<Collider>();
        }
        if (item == null)
        {
            item = GetComponent<Item>();
            item.dropItem = this.gameObject;
            go = gameObject;
        }
        item.dropItem = this.gameObject;
        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        Init();
        StartCoroutine(EnableColliderAfterDelay(_collider, 0.1f));
    }

    IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }


    public void SetItem(Item item)
    {
        Debug.Log("test");
        Debug.Log(item);
        this.item = item;
        Debug.Log(this.item);
        item.dropItem = this.gameObject;
    }
    public void SetModel()
    {
        if (item == null)
        {
            Debug.LogError($"{this.gameObject.name} is null");
            Init();
        }
    }
    public void DestoryItem()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터 클라이언트에게 삭제 요청");
            photonView.RPC("DestroyObjectRPC", RpcTarget.MasterClient);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"collider {item.name}");
        if (other.CompareTag("Player"))
        {

            //InventoryController.Instance.CreateDropItemToInventory(item);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!gameObject.activeSelf) { return; }
            //item.itemIcon.GetComponent<ItemIcon>().NullItemGrid();
            //InventoryController.Instance.RemoveItemIcon(item);
        }
    }*/

    public void ActiveCollider(bool value)
    {
        _collider.enabled = value;
    }
    public void SetDisableRPC()
    {
        photonView.RPC("DisableItemRPC", RpcTarget.All, photonView.ViewID);
    }

    int CheckViewID()
    {
        return photonView.ViewID;
    }

    [PunRPC]
    void DestroyObjectRPC()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            Debug.Log("마스터 클라이언트가 아이템 삭제");
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
