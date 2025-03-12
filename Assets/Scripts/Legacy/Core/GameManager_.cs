using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class GameManager_ : MonoBehaviourPunCallbacks
{
    public static GameManager_ Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager_>();

            return instance;
        }
    }

    private static GameManager_ instance;

    public Transform[] spawnPositions;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera virtualCamera;


    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var localPlayerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var spawnPosition = spawnPositions[localPlayerIndex % spawnPositions.Length];


        GameObject player = PhotonNetwork.Instantiate("Prefabs/Player/Player", spawnPosition.position, Quaternion.identity);
        InventoryController.Instance.SetPlayer(player.GetComponent<PlayerTrigger>());
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }

    public override void OnLeftRoom()
    {
        SceneController.Instance.LoadScene("Lobby");
    }
}