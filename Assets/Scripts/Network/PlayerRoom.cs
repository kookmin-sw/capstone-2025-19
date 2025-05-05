using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerRoom : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roomTitle;
    [SerializeField] TextMeshProUGUI masterClientName;
    [SerializeField] Image lockImage;



    [HideInInspector] public RoomInfo roomInfo;
    [HideInInspector] public bool lockValue = false;
    [HideInInspector] public string pwd;

    



    public void SetMasterClientName(string name)
    {
        masterClientName.text = name;
    }
    public void SetRoomTitle(string title)
    {
        roomTitle.text = title; 
    }

    public void SetLoackImage(bool value)
    {
        lockImage.gameObject.SetActive(value);
        lockValue = value;
    }


    public void EnterRoom()
    {
        if (!roomInfo.IsOpen || !roomInfo.IsVisible) { Debug.LogError($"지금 {roomInfo.Name}방에 입장 할 수 없습니다."); return; }
        Debug.Log("Button press");
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("is not connected and ready");
            PhotonNetwork.ConnectUsingSettings();
            return;
        }

        if(roomInfo.CustomProperties.TryGetValue("pwd", out object pwdObj))
        {
            //TODO Password panel
            NetworkController.Instance.EnterPrivateRoom(roomInfo);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            NetworkController.Instance.EnterOtherPlayerRoom(roomInfo);
        }

    }

    

    
}
