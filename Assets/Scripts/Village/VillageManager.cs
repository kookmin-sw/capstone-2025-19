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
        Debug.Log("LoginCanvas ����� �Ϸ�");
    }

    public void SpawnPlayer()
    {
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer_Village"));
        
        player.transform.position = playerSpawnPosition.position;
        InventoryController.Instance.SetPlayer(player.GetComponent<PlayerTrigger>());
        //TODO Player MainCamera ����
        //GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        playerFollowCamera = Instantiate(Resources.Load<GameObject>("Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = playerFollowCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform.Find("PlayerCameraRoot");

        player.GetComponent<PlayerController>().SetMainCamera(mainCamera);
        //ī�޶� ĳ���Ϳ��Է� ȸ��
        OnLoginComplete(); 
        CloseLoginCanvas();
    }

    public void OnLoginComplete()
    {
        // 1) �÷��̾� ī�޶� �켱������ ����
        playerFollowCamera.GetComponent<CinemachineVirtualCamera>().Priority = 11;

        // 2) �α��� ī�޶�� �켱������ ���߰ų�
        loginVirtualCamera.Priority = 1;
    }
}
