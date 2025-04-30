using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DungeonGenerator_M : DungeonGenerator
{
    public override void StartGeneration()
    {
        Debug.Log("Photon test1");
        //Debug.Log($"SceneManager name = {SceneController.Instance.GetCurrentSceneName()}");
        if (PhotonNetwork.IsMasterClient)
        {

            //TODO Host createRoom
            Generate_MultiPlay();
            //StartCoroutine(GenerateMultiplay());
            NvigationBake();
            PlayerSpawn();
            SpawnRandomObject();
        }
        else
        {
            //Waiting host createRoom
            //SpawnPlayer()
            networkEventReceiver.RequestPlayerSpawn();
            StartCoroutine(WaitHostReady());
        }
        NvigationBake();
        SpawnRandomObject();
        PlayerSpawn();
        isGenerated = true;
    }



    protected override IEnumerator WaitOtherPlayerEnter(GameObject player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            while (!networkEventReceiver.playerSpawnReady)
            {
                yield return new WaitForSeconds(1f);
            }
        }


        InventoryController.Instance.SetPlayer(player.transform.Find("Trigger").GetComponent<PlayerTrigger>());
        playerSpawnDungeonPart.roomUse = DungeonPart.RoomUse.PlayerSpawn;
        Debug.Log(player.transform.position);
        player_ = player;

        networkEventReceiver.SendAllPlayerReady();
        StartCoroutine(ResetPlayerPosition());
    }

    protected override void StartGenerationServerRpc()
    {
        Generate_MultiPlay();
    }

    protected override void PlayerSpawn()
    {
        int tryCount = 0;
        while (playerSpawnDungeonPart == null || playerSpawnDungeonPart.dungeonPartType == DungeonPart.DungeonPartType.SpecialRoom)
        {
            //무한루프 빠질 가능성 높음 
            //무한루프 빠질 경우 각 방의 플레이어 스폰 리스트 확인하기
            playerSpawnDungeonPart = generatedRooms[UnityEngine.Random.Range(0, generatedRooms.Count)];
            Debug.Log(playerSpawnDungeonPart.dungeonPartType);
            if (playerSpawnDungeonPart.playerSpawnPoints.Count <= 0) { playerSpawnDungeonPart = null; }
            tryCount += 1;
            if (tryCount > 100) { break; }
        }
        Transform playerSpawnPosition = playerSpawnDungeonPart.playerSpawnPoints[0];
        GameObject player;
        
        player = PhotonNetwork.Instantiate("Prefabs/Player/Player_Multiplay", playerSpawnPosition.position, Quaternion.identity);
        //Wait other player before connect InventoryController
        networkEventReceiver.PlusPlayer();
        StartCoroutine(WaitOtherPlayerEnter(player));
    }
}
