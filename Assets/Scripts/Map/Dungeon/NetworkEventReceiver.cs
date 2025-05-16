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
    [SerializeField] public int playerCount = 1;
    [HideInInspector] public int courrentPlayerCount = 0;
    private Dictionary<byte, Action<EventData>> handlers;
    public bool playerSpawnReady = false;
    

    private void Awake()
    {
        handlers = new Dictionary<byte, Action<EventData>>
        {
            { (byte)NetworkEventCode.RequestPlayerSpawn, GetSpawnPosition },
            { (byte)NetworkEventCode.SendPlayerSpawnPosition, ReceivePosition},
            { (byte)NetworkEventCode.CountClientPlayer, CountClientPlayer },
            { (byte)NetworkEventCode.ReceiveReady, ChangePlayerReady },
            { (byte)NetworkEventCode.ClientReady,CountClientPlayer  },
            { (byte)NetworkEventCode.BossDie,CountBossKill  },

        };
    }
    public enum NetworkEventCode : byte
    {
        RequestPlayerSpawn = 1,
        SendPlayerSpawnPosition = 2,
        CountClientPlayer = 3,
        ReceiveReady = 4,
        ClientReady = 5,
        BossDie = 6,

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
        Debug.Log("WaitUntilSpawnReady");
        while (!DungeonGenerator.Instance.IsGenerated())
        {
            Debug.Log("Waiting");
            yield return new WaitForSeconds(1f);
        }

        Vector3 position = DungeonGenerator.Instance.GetPlayerSpawnPosition(actorNumber);
        SendPlayerPosition(actorNumber, position);
    }

    private void SendPlayerPosition(int actorNumber, Vector3 position)
    {
        Debug.Log($"SendPlayerPosition {position}");
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
        Debug.Log("RequestPlayerspawn");
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
        Debug.Log($"Receive position {position}");
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            DungeonGenerator.Instance.SpawnPlayer_Multiplay(position);
        }
    }

    public void CountClientPlayer(EventData photonEvent)
    {
        courrentPlayerCount++;
        if(courrentPlayerCount == playerCount) { 
            playerSpawnReady = true; 
            
        }
    }

    public void PlusPlayer()
    {
        courrentPlayerCount++;
        if (courrentPlayerCount == playerCount) { playerSpawnReady = true; }
    }

    public void SendAllPlayerReady()
    {
        Debug.Log("Player Ready!!");
        PhotonNetwork.RaiseEvent(
            (byte)NetworkEventReceiver.NetworkEventCode.ReceiveReady,
            PhotonNetwork.LocalPlayer.ActorNumber, // 클라이언트 식별용
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable
        );
    }

    public void SendThisPlayerReady()
    {
        Debug.Log("This player ");
        PhotonNetwork.RaiseEvent((byte)NetworkEventReceiver.NetworkEventCode.ClientReady,
            PhotonNetwork.LocalPlayer.ActorNumber,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            SendOptions.SendReliable
            );
    }

    private void ChangePlayerReady(EventData photonEvent)
    {
        playerSpawnReady = true;
    }

    private void CountBossKill(EventData photonEvent)
    {

    }

    IEnumerable WaitForplayerReady()
    {
        while(playerSpawnReady == false)
        {
            yield return new WaitForSeconds(1f);
        }
        DungeonGenerator.Instance.ClientPlayerSpawn();

    }
    
}
