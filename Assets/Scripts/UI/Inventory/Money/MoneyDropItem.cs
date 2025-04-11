using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MoneyDropItem : MonoBehaviour
{
    public GameObject MoneyItemIcon;
    public int moneyValue = 1;
    PhotonView photonView;
    // Start is called before the first frame update
    void Start()
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

    public void SetValue(int moneyValue)
    {
        if(SceneController.Instance.GetCurrentSceneName() == "Dungeon_Multiplay")
        {
            if (PhotonNetwork.IsMasterClient)//마스터 클라이언트일 경우
            {
                UpdateValue(moneyValue);
                DebugText.Instance.Debug($"Set Item {moneyValue} masterClient");
                photonView.RPC("UpdateItemPhoton", RpcTarget.OthersBuffered, moneyValue);
            }
            else// 기본 클라이언트일 경우
            {
                DebugText.Instance.Debug($"Set item {moneyValue} client");
                photonView.RPC("RequestUpdateItemPhoton", RpcTarget.MasterClient, moneyValue);//마스터 클라이언트에게 변경 요청
            }
        }
    }
    [PunRPC]
    public void RequestUpdateItemPhoton(int moneyValue)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DebugText.Instance.Debug($"itemData requestUpdateItemPhoton {moneyValue}");
            //string itemData = ChangeData(item); // 데이터를 문자열로 변환
            photonView.RPC("UpdateItemPhoton", RpcTarget.AllBuffered, moneyValue);
        }
    }
    [PunRPC]
    private void UpdateItemPhoton(int moneyValue)
    {

        this.moneyValue = moneyValue;
    }


    private void UpdateValue(int moneyValue)
    {
        this.moneyValue = moneyValue;
    }
    public void RemoveDropItem()
    {
        MoneyItemIcon = null;
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
            //InventoryController.Instance.dropItemList.Remove(this);
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
        if (targetView != null && targetView.IsMine) //  소유자만 실행
        {
            photonView.RPC("DeactivateDropItem", RpcTarget.AllBuffered, viewID);
        }
    }

}
