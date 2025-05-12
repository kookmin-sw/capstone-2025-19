using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : Singleton<NetworkController>
{
    private NetworkCallback networkCallback;
    [Header("LoadSceneName")]
    [SerializeField] private string loadSceneName;

    [Space(20)]
    [Header("Server Rate")]
    [SerializeField] private int sendRate = 30;
    [SerializeField] private int serializationRate = 20;


    [Space(20)]

    [Header("SceneName")]
    [SerializeField] private string singleplaySceneName;
    [SerializeField] private string multiplaySceneName;

    [Space(10)]
    
    [Header("PlayModeSelectButton")]
    [SerializeField] private GameObject playModeSelectPanel;
    [SerializeField] private Button singlePlayButton;
    [SerializeField] private Button multiPlayButton;

    [Space(10)]

    [Header("MultiPlayRoom")]
    [SerializeField] private GameObject multiplayRoomPanel;
    [SerializeField] private GameObject createRoomSettingPanel;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private ScrollRect roomScrollRect;
    [SerializeField] private Button backspaceButton;
    [SerializeField] private PasswordPanel passwordPanel;

    [Space(10)]

    [Header("Player Ready Room")]
    [SerializeField] public GameObject roomPanel;
    [SerializeField] private Transform readyPlayerPanel;
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private GameObject playerProfilePrefab;





    [HideInInspector] public int playerCount;
    protected override void Awake()
    {
        base.Awake();
        AllPanelActiveFalse();
        playModeSelectPanel.SetActive(true);

        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializationRate;
    }
    public void AllPanelActiveFalse()
    {
        playModeSelectPanel.SetActive(false);
        multiplayRoomPanel.SetActive(false);
        createRoomSettingPanel.SetActive(false);
        roomPanel.SetActive(false);
        passwordPanel.gameObject.SetActive(false);
    }

    public void EnterSinglePlayScene()
    {
        SceneController.Instance.LoadScene(singleplaySceneName);
    }

    public void EnterPhotonServer()
    {
        AllPanelActiveFalse();
        multiplayRoomPanel.SetActive(true);
        networkCallback = gameObject.AddComponent<NetworkCallback>();
        networkCallback.JoinedServer();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
        AllPanelActiveFalse();
        playModeSelectPanel.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        ClearPlayerPrfilePanel();
        AllPanelActiveFalse();
        multiplayRoomPanel.SetActive(true) ;
    }
    public void CreateRoomButton()
    {
        createRoomSettingPanel.SetActive(true);
    }

    public void CreateRoom(string roomName, bool setPrivate, string pwd = "")
    {
        //TODO Firbase Player Name
        //networkCallback.CreateRoom(roomName,InventoryController.Instance.player.name, setPrivate, pwd);

        networkCallback.CreateRoom(roomName, PlayerStatusController.Instance.playerNickname, setPrivate, pwd);
        networkCallback.CreateRoom(roomName,"TestPlayerName", setPrivate, pwd);
        Debug.Log($"Create room {roomName}");
        AllPanelActiveFalse();
        roomPanel.SetActive(true);
        this.roomName.text = roomName;  
    }

    public void SetNickName()
    {
        PhotonNetwork.NickName = PlayerStatusController.Instance.playerNickname;
    }


    
    public void StartLevel()
    {
        //TODO Load MultiplayScene
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        PhotonNetwork.LoadLevel(multiplaySceneName);
        AllPanelActiveFalse();

    }


    public void RemoveRoomUI(RoomInfo room)
    {
        Debug.Log($"Remove room ui {room}");
        if (!networkCallback.roomDictionary.ContainsKey(room.Name)) { return; }
        PlayerRoom playerRoom = networkCallback.roomDictionary[room.Name];
        networkCallback.roomDictionary.Remove(room.Name);
        Destroy(playerRoom.gameObject);
    }
    public PlayerRoom CreateRoomUI(RoomInfo room)
    {
        Debug.Log($"CreateRoom ui {room.Name}");
        GameObject roomIcon = Instantiate(roomPrefab);
        PlayerRoom playerRoom = roomIcon.GetComponent<PlayerRoom>();
        playerRoom.SetRoomTitle(room.Name);
        if(room.CustomProperties.TryGetValue("PlayerName", out object playerName))
        {
            Debug.Log($"client name is : {playerName.ToString()}");
            playerRoom.SetMasterClientName(playerName.ToString());
        }
        if(room.CustomProperties.TryGetValue("pwd", out object password))
        {
            Debug.Log($"pwd : {password.ToString()}");
            playerRoom.SetLoackImage(true);
            playerRoom.pwd = password.ToString();
        }
        else { playerRoom.SetLoackImage(false); }
        playerRoom.roomInfo = room;

        roomIcon.transform.SetParent(roomScrollRect.content);
        return playerRoom;
    }

    public bool CheckSameRoomName(string roomName)
    {
        return networkCallback.roomDictionary.ContainsKey(roomName);
    }
    

    public void EnterOtherPlayerRoom(RoomInfo room)
    {
        AllPanelActiveFalse();
        roomPanel.SetActive(true);
        roomName.text = room.Name;
        Debug.Log("EnterOtherPlayerRoom");
        PhotonNetwork.JoinRoom(room.Name);
    }

    public void EnterPrivateRoom(RoomInfo room)
    {
        passwordPanel.gameObject.SetActive(true);
        passwordPanel.SetRoomInfo(room);
    }

    public void AddPlayerPanel(Player player)
    {
        if (networkCallback._playerPanels.ContainsKey(player.ActorNumber)) return;

        GameObject go = Instantiate(playerProfilePrefab, readyPlayerPanel);
        PlayerProfilePanel profilePanel = go.GetComponent<PlayerProfilePanel>();
        profilePanel.SetPlayer(player.NickName, player.ActorNumber);
        networkCallback._playerPanels[player.ActorNumber] = go;
        
    }

    public void RemovePlayerPanel(Player player)
    {
        if (!networkCallback._playerPanels.ContainsKey(player.ActorNumber)) return;

        GameObject go = networkCallback._playerPanels[player.ActorNumber];
        networkCallback._playerPanels.Remove(player.ActorNumber);

        Destroy(go);
    }

    private void ClearPlayerPrfilePanel()
    {
        networkCallback._playerPanels.Clear();
        foreach(Transform child in readyPlayerPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void EnterVillage()
    {
        PhotonNetwork.LeaveRoom();
        SceneController.Instance.LoadScene("Village");
    }

}




