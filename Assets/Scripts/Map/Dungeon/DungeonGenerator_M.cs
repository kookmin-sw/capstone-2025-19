using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DungeonGenerator_M : DungeonGenerator
{
    [SerializeField] private string entranceRoomName;
    PhotonView photonView;
    public override void StartGeneration()
    {
        photonView = GetComponent<PhotonView>();
        Debug.Log("Photon test1");
        //Debug.Log($"SceneManager name = {SceneController.Instance.GetCurrentSceneName()}");
        NetworkController.Instance.AllPanelActiveFalse();
        if (PhotonNetwork.IsMasterClient)
        {
            networkEventReceiver.playerCount = NetworkController.Instance.playerCount;
            //TODO Host createRoom
            Generate_MultiPlay();
            FillWall();
            //StartCoroutine(GenerateMultiplay());
            NvigationBake();
            PlayerSpawn();
            SpawnRandomObject();
            for(int i = 0; i < potalCount; i++)
            {
                SetEscapePotal();
            }
            
        }
        else
        {
            //Waiting host createRoom
            //SpawnPlayer()
            networkEventReceiver.RequestPlayerSpawn();
            StartCoroutine(WaitHostReady());
        }
        /*NvigationBake();
        SpawnRandomObject();
        PlayerSpawn();*/
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
        InventoryController.Instance.SetPlayerController(player.GetComponent<PlayerControl.PlayerController>());
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
            //���ѷ��� ���� ���ɼ� ���� 
            //���ѷ��� ���� ��� �� ���� �÷��̾� ���� ����Ʈ Ȯ���ϱ�
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

    protected void Generate_MultiPlay()
    {
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                //���� ������ ó�� ���� ���� �÷��̾� ���� ����
                Debug.Log("Photon test");
                GameObject generatedRoom = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/Entrance/{entranceRoomName}", transform.position, transform.rotation);
                //GameObject generatedRoom = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Entrance/{entranceRoomName}", transform.position, transform.rotation);
                //GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //���⿡ �ִ� entrance�� ������ �������� �ɰ���
                generatedRoom.transform.SetParent(null);
                //��Ƽ�÷��� �����
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //dungeonPart.GetNetworkObject().Spawn(true); //��� Ŭ���̾�Ʈ�� ���� ���� ��ü�� �����ϴ� 
                    generatedRooms.Add(dungeonPart); //�� ����� List�� �߰�
                }
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                //bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.6f; //������ ������ Ȯ�� 50%
                DungeonPart room1 = null; //�̹� ���� ���ο� �������� ������ ��
                Transform room1EntryPoint = null; // ������ ������ ���� �������� �� ������.
                                                  // �⺻������ ��A�� ��B�� ���� �� �� A�� ��B�� ���� �� �� �ֱ⿡ �� ��A(���� ��)�� ������(�Ա�, ��)�� ��Ÿ���� ����Ʈ

                int totalRetries = 100; //��ü ��Ʈ���� Ƚ��. ������ġ��µ�
                int retryIndex = 0;

                if (retryIndex > totalRetries) { Debug.LogError("Create room error!!"); break; }
                int ramdomGenerateRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count);
                room1 = generatedRooms[ramdomGenerateRoomIndex];
                retryIndex += 1;
                if (room1.HasAvailableEntryPoint(out room1EntryPoint))
                {
                    DungeonPart room2 = null;

                    EntryPoint.NeedRoomType value = EntryPoint.NeedRoomType.None;
                    foreach (var dictionary in room1EntryPoint.GetComponent<EntryPoint>().needRoomArray)
                    {
                        float randomValue_ = UnityEngine.Random.Range(0f, 1);
                        if (dictionary.Value >= randomValue_)
                        {
                            value = dictionary.Key;


                        }
                    }
                    if (value == EntryPoint.NeedRoomType.None) { i--; continue; }
                    switch (value)
                    {
                        case EntryPoint.NeedRoomType.Stair:
                            int randomStairIndex = UnityEngine.Random.Range(0, stairs.Count);
                            Debug.Log($"random value test {randomStairIndex},{stairs.Count}");
                            room2 = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/Stairs/{stairs[randomStairIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();
                            /*room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Stairs/{stairs[randomStairIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();*/

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomHallwaysIndex = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/Hallways/{hallways[randomHallwaysIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.Room:
                            int randomRoomsIndex = UnityEngine.Random.Range(0, rooms.Count);
                            room2 = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/Rooms/{rooms[randomRoomsIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.SmallRoom:
                            int randomSmallRoomIndex = UnityEngine.Random.Range(0, smallRooms.Count);
                            room2 = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/SmallRooms/{smallRooms[randomSmallRoomIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.TrapRoom:
                            room2 = PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/TrapMap/TrapMap_M", transform.position, transform.rotation).GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                            /*case EntryPoint.NeedRoomType.None:
                                int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                                int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                                Debug.Log("None test");
                                room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/AllRooms/{allRoomsModules[randomRoom2Index_][randomRoom2Index__]}"
                                    , transform.position, transform.rotation).GetComponent<DungeonPart>();

                                if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                                break;*/


                    }


                    /*switch (room1EntryPoint.GetComponent<EntryPoint>().needRoomType)
                    {
                        case EntryPoint.NeedRoomType.Stair:
                            int randomStairIndex = UnityEngine.Random.Range(0, stairs.Count);
                            Debug.Log($"random value test {randomStairIndex},{stairs.Count}");
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Stairs/{stairs[randomStairIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomHallwaysIndex = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Hallways/{hallways[randomHallwaysIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.Room:
                            int randomRoomsIndex = UnityEngine.Random.Range(0, rooms.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Rooms/{rooms[randomRoomsIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                        case EntryPoint.NeedRoomType.None:
                            int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                            int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                            Debug.Log("None test");
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/AllRooms/{allRoomsModules[randomRoom2Index_][randomRoom2Index__]}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); i--; continue; }
                            break;
                    }*/
                    generatedRooms.Add(room2);
                    if (room2.dungeonPartType == DungeonPart.DungeonPartType.Hallway || room2.dungeonPartType == DungeonPart.DungeonPartType.Stair) { i--; continue; }
                }
            }
        }
        Debug.Log("Generated room is finish");
        isGenerated = true;
    }

    protected override void FillWall()
    {
        foreach(DungeonPart dungeonPart in generatedRooms)
        {
            foreach(Transform entryPoint_ in dungeonPart.entryPoints)
            {
                EntryPoint entryPoint = entryPoint_.GetComponent<EntryPoint>();
                if (!entryPoint.IsOccupied()) 
                {

                    PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/FillerWall/{entryPoint.wallObject.name}", entryPoint.entrance.transform.position, entryPoint.entrance.transform.rotation);
                    PhotonView entryPhoton = entryPoint.entrance.gameObject.GetComponent<PhotonView>();
                    Debug.Log(entryPoint.entrance.gameObject);
                    Debug.Log($"is null? {entryPhoton.ViewID}");
                    photonView.RPC(nameof(DestoryWall), RpcTarget.All, entryPhoton.ViewID);

                }
            }
        }
    }

    public override void EnterBossRoom()
    {
        
    }

    protected override void SetEscapePotal()
    {
        int totalTryCount = 100;
        int tryCount = 0;
        DungeonPart escapeRoom = null;
        while (tryCount < totalTryCount && escapeRoom == null)
        {
            int randomIndex = UnityEngine.Random.Range(0, generatedRooms.Count);
            escapeRoom = generatedRooms[randomIndex];
            if (escapeRoom.roomUse == DungeonPart.RoomUse.PlayerSpawn ||
                escapeRoom.dungeonPartType == DungeonPart.DungeonPartType.Hallway ||
                escapeRoom.dungeonPartType == DungeonPart.DungeonPartType.BossRoom )
            {
                escapeRoom = null;
                tryCount++;
                continue;
            }
            if (escapeRoom.escapePotalSpawnPoints.Count == 0) { escapeRoom = null; tryCount++; continue; }
            int randomEscapeIndex = UnityEngine.Random.Range(0, escapeRoom.escapePotalSpawnPoints.Count);
            if (escapeRoom.escapePotalSpawnPoints[randomEscapeIndex].isUse) { escapeRoom = null; tryCount++; continue; }
            escapeRoom.escapePotalSpawnPoints[randomEscapeIndex].isUse = true;
            PhotonNetwork.InstantiateRoomObject($"Prefabs/Map/MultiPlay/Potal/EscapePotal",
                escapeRoom.escapePotalSpawnPoints[randomEscapeIndex].transform.position, escapeRoom.escapePotalSpawnPoints[randomEscapeIndex].transform.rotation);
            break;
        }
    }

    [PunRPC]
    private void DestoryWall(int childViewID)
    {
        GameObject entranceGO = PhotonView.Find(childViewID).gameObject;
        entranceGO.SetActive(false);
    }



}
