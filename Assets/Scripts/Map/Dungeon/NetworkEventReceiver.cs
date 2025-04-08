using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static DungeonGenerator;

public class NetworkEventReceiver : MonoBehaviour, IOnEventCallback
{
    private Dictionary<byte, Action<EventData>> handlers;

    private void Awake()
    {
        handlers = new Dictionary<byte, Action<EventData>>
        {
            { (byte)NetworkEventCode.RequestPlayerSpawn, GetSpawnPosition },
            { (byte)NetworkEventCode.SendPlayerSpawnPosition, ReceivePosition},
        };
    }
    public enum NetworkEventCode : byte
    {
        RequestPlayerSpawn = 1,
        SendPlayerSpawnPosition = 2,
    }
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    public void OnEvent(EventData photonEvent)
    {
        if(handlers.TryGetValue(photonEvent.Code, out var handler))handler.Invoke(photonEvent);
        
    }

    private void GetSpawnPosition(EventData photonEvent)
    {
        int actorNumber = (int)photonEvent.CustomData;
        
        StartCoroutine(WaitUntilSpawnReady(actorNumber));
    }

    private IEnumerator WaitUntilSpawnReady(int actorNumber)
    {
        while (!DungeonGenerator.Instance.IsGenerated())
        {
            yield return new WaitForSeconds(1f);
        }

        Vector3 position = DungeonGenerator.Instance.GetPlayerSpawnPosition(actorNumber);
        SendPlayerPosition(actorNumber, position);
    }

    private void SendPlayerPosition(int actorNumber, Vector3 position)
    {
        object[] objects = new object[] { actorNumber, position };
        PhotonNetwork.RaiseEvent(
            (byte)NetworkEventReceiver.NetworkEventCode.SendPlayerSpawnPosition,
            objects, // 클라이언트 식별용
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    public void RequestPlayerSpawn()
    {
        PhotonNetwork.RaiseEvent(
            (byte)NetworkEventReceiver.NetworkEventCode.RequestPlayerSpawn,
            PhotonNetwork.LocalPlayer.ActorNumber, // 클라이언트 식별용
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            SendOptions.SendReliable
        );
    }

    private void ReceivePosition(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;
        int actorNumber = (int)data[0];
        Vector3 position = (Vector3)data[1];
        if(actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DungeonGenerator.Instance.SpawnPlayer_Multiplay(position);
        }
    }
    
}
