using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static DungeonGenerator;

public class NetworkEventReceiver : MonoBehaviour, IOnEventCallback
{
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    public void OnEvent(EventData photonEvent)
    {
        if (PhotonNetwork.IsMasterClient)
        {

        }
        else
        {

        }
        /*if (photonEvent.Code == (byte)NetworkEventCode.RequestPlayerSpawn)
        {
            int actorNumber = (int)photonEvent.CustomData;
            //Vector3 spawnPos = DungeonGenerator.Instance.GetAvailableSpawnPosition();

            object[] data = new object[] { actorNumber, spawnPos };

            PhotonNetwork.RaiseEvent(
                (byte)NetworkEventCode.SendPlayerSpawnPosition,
                data,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable
            );
        }*/
    }
}
