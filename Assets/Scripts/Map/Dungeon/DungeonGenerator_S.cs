using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DungeonGenerator_S : DungeonGenerator
{
    public override void StartGeneration()
    {
        StartGenerationServerRpc();
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
        player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/Player_SinglePlay"), playerSpawnPosition.position, Quaternion.identity);
        //Wait other player before connect InventoryController
        networkEventReceiver.PlusPlayer();
        StartCoroutine(WaitOtherPlayerEnter(player));
    }

    protected override void StartGenerationServerRpc()
    {
        StartCoroutine(GenerateCoroutine());
        /*Generate();

        NvigationBake();
        SpawnRandomObject();
        PlayerSpawn();
        isGenerated = true;*/
    }

    protected override IEnumerator WaitOtherPlayerEnter(GameObject player)
    {
        throw new System.NotImplementedException();
    }


}
