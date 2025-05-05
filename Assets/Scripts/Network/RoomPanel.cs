using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Transform readyPlayerPanel;
    [SerializeField] GameObject playerPanelPrefab;

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(false);
        }
        else { startButton.gameObject.SetActive(true);}
    }

    public void StartButton()
    {
        NetworkController.Instance.StartLevel();
    }
}
