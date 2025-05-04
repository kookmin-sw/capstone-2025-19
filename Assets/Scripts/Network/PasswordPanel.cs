using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasswordPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TMP_InputField password;
    RoomInfo info;
    
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;
    }



    public void EnterRoomButton()
    {
        if(info.CustomProperties.TryGetValue("pwd", out object pwd))
        {
            if(pwd.ToString() == password.text)
            {
                NetworkController.Instance.EnterOtherPlayerRoom(info);
            }
            else { Debug.Log("Uncorrected password!!"); }

        }
        else { Debug.LogError("Bug"); }
    }

    public void CancelButton()
    {
        gameObject.SetActive(false);
    }
    
}
