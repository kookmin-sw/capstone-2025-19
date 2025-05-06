using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Transform readyPlayerPanel;
    [SerializeField] GameObject playerPanelPrefab;

    

    public void SetRoom()
    {
        Debug.Log("start button test");
        Debug.Log($"Master client {PhotonNetwork.IsMasterClient}");
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
        }
        else { startButton.gameObject.SetActive(false); }
    }

    public void StartButton()
    {
        NetworkController.Instance.StartLevel();
    }
}
