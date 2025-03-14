using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerControl;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void SpawnPlayer()
    {
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer_Village"));
        player.transform.position = playerSpawnPosition.position;
        InventoryController.Instance.SetPlayer(player.GetComponent<PlayerTrigger>());
        //TODO Player MainCamera 생성
        GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        GameObject playerFollowCamera = Instantiate(Resources.Load<GameObject>("Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform.Find("PlayerCameraRoot");

        player.GetComponent<PlayerController>().SetMainCamera(mainCamera);
        //TODO Firebase 정보 받아서 Player 장비 최신화
    }
}
