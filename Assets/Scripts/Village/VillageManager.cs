using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using PlayerControl;
using UnityEngine;

public class VillageManager : MonoBehaviour
{
    [SerializeField] Transform playerSpawnPosition;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject LoginCanvas;

    [SerializeField] CinemachineVirtualCamera loginVirtualCamera;
    [HideInInspector]
    public GameObject playerFollowCamera;

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CloseLoginCanvas()
    {
        LoginCanvas.SetActive(false);
        Debug.Log("LoginCanvas 지우기 완료");
    }

    public void SpawnPlayer()
    {
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer_Village"));
        
        player.transform.position = playerSpawnPosition.position;
        InventoryController.Instance.SetPlayer(player.GetComponent<PlayerTrigger>());
        //TODO Player MainCamera 생성
        //GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        playerFollowCamera = Instantiate(Resources.Load<GameObject>("Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform.Find("PlayerCameraRoot");

        player.GetComponent<PlayerController>().SetMainCamera(mainCamera);
        //카메라 캐릭터에게로 회전
        OnLoginComplete(); 
        CloseLoginCanvas();
    }

    public void OnLoginComplete()
    {
        // 1) 플레이어 카메라 우선순위를 높게
        playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11;

        // 2) 로그인 카메라는 우선순위를 낮추거나
        loginVirtualCamera.Priority = 1;
    }
}
