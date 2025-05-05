using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class NetworkCallback : MonoBehaviourPunCallbacks
{
    public Dictionary<string, PlayerRoom> roomDictionary = new Dictionary<string, PlayerRoom>();
    private readonly string gameVersion = "1"; // 게임 버전

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                Debug.Log($"Remove room {info.Name}");
                NetworkController.Instance.RemoveRoomUI(info);
                continue;
            }
            else if(!roomDictionary.ContainsKey(info.Name))
            {

                roomDictionary.Add(info.Name,NetworkController.Instance.CreateRoomUI(info));
            }
            else
            {
                Debug.Log($"Someon enter the room {info.Name}");
                Debug.Log($"current player : {info.PlayerCount}  maxPlayer : {info.MaxPlayers}");
            }
        }
    }

    public void JoinedServer()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    public void CreateRoom(string name, string playerName, bool isPrivate, string pwd)
    {
        if (!PhotonNetwork.IsConnectedAndReady) { Debug.LogError($"Don't connected Lobby"); return; }

        if (isPrivate)
        {
            var options = new RoomOptions
            {
                // 커스텀 룸 프로퍼티에 "pwd" 키로 비밀번호 저장
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "PlayerName", playerName }, { "pwd", pwd }, },

                // 로비에서 이 프로퍼티를 RoomInfo.CustomProperties에 포함시킬지 여부
                CustomRoomPropertiesForLobby = new string[] { "PlayerName", "pwd" }
            };
            PhotonNetwork.CreateRoom(name, options);
        }
        else
        {
            var options = new RoomOptions
            {
                CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "PlayerName", playerName } },

                CustomRoomPropertiesForLobby = new string[] { "PlayerName" }
            };
            PhotonNetwork.CreateRoom(name, options);
        }


        Debug.Log("CreateRoom test");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecting");
        PhotonNetwork.JoinLobby();
    }

    private void Start()
    {
        Debug.Log("test");
    }
}
