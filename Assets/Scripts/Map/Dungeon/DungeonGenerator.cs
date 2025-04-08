using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;


public class DungeonGenerator : Singleton<DungeonGenerator>
{
    [Header("���� ���� ���")]
    [SerializeField, Tooltip("������ �� ����")] private int noOfRooms = 10;
    [Space(10)]
    [SerializeField, Tooltip("�������� ��")] private GameObject entrance; //������ �������� ��
    [SerializeField, Tooltip("�Ϲ����� ��")] private List<GameObject> rooms;
    [SerializeField, Tooltip("���")] private List<GameObject> stairs;
    [SerializeField, Tooltip("Ư���� ��(���� ������ ���� �� �ϳ� �̻� ���� �ȵ�. ex) �������� �� ������)")] private List<GameObject> specialRooms; //����ǰ�� ���� �� �ִ� Ư���� ��
    [SerializeField, Tooltip("��ü �Ա� -> �Ⱦ��Ŵ� ������ �ȵ�")] private List<GameObject> alternateEntrances; //��ü �Ա� ex)���� ���۴��� ��� -> �ʿ� ���� ��
    [SerializeField, Tooltip("����. ������ �� ī��Ʈ�� ���� �ȵ�")] private List<GameObject> hallways;
    [SerializeField, Tooltip("��. ���� �۵� ����")] private GameObject door;

    private List<List<GameObject>> allRoomsModules;
    //���󿡼� �� ������Ʈ�� ���簡 ���� ������ ���� �ʵ��� y�� ���� -1000�� ���� �ƴѰ�? ���� �ؼ��� �߸��߳�?
    [Space(10)]
    [Header("�����Ŵ��� ����, �ٲ�� �ȵ�")]
    [SerializeField] LayerMask roomsLayerMask;
    [Space(10)]
    [Header("�׽�Ʈ �ν�����")]
    [SerializeField] GameObject dontSelectedEntryGO;
    
    private List<DungeonPart> generatedRooms; //���� ���ο� ������ ��List
    //private List<EntryPoint> emtryList;
    private bool isGenerated = false;


    private DungeonPart playerSpawnDungeonPart;
    NetworkEventReceiver networkEventReceiver;

    [HideInInspector] public NavMeshSurface surface;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        networkEventReceiver = GetComponent<NetworkEventReceiver>();
    }
    void Start()
    {
        //Add room list;
        SceneController.Instance.SubscribeGo(this.gameObject);
        allRoomsModules = new List<List<GameObject>>();
        allRoomsModules.Add(rooms);
        allRoomsModules.Add(stairs);
        allRoomsModules.Add(hallways);


        generatedRooms = new List<DungeonPart>();
        surface = GetComponent<NavMeshSurface>();
        //StartCoroutine(Generated_());
        StartGeneration();
    }
    //���� ����
    public void StartGeneration()
    {
        if(SceneController.Instance.GetCurrentSceneName() == "Multiplay")
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //TODO Host createRoom
                Generate_MultiPlay();
                PlayerSpawn();
            }
            else
            {
                //Waiting host createRoom
                //SpawnPlayer()
                networkEventReceiver.RequestPlayerSpawn();
                NvigationBake();
            }
        }
        else
        {
            NvigationBake();
            Generate();
            SpawnRandomObject();
            PlayerSpawn();
        }
        
    }


    //[ServerRpc(RequireOwnership = false)]
    private void StartGenerationServerRpc()
    {
        if(SceneController.Instance.GetCurrentSceneName() == "MultiplayRoomTest")
        {
            //TODO PhotonView Instance
            Generate_MultiPlay();
            //FillEmptyEntrances_MultiPlay();
        }
        else
        {
            Generate(); //���� ����
            //GenerateAlternateEntrances(); //�ٸ� �Ա� ���� -> �ϴ� ����. �˰��� ��ü�� ���� �� ������ �Ȱ���
            //FillEmptyEntrances(); //��� ���� ������ ���� ���� �Ա� ������ ����
        }

        NvigationBake();
        SpawnRandomObject();
        PlayerSpawn();
        isGenerated = true;
    }

    private void Generate_MultiPlay()
    {
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                //���� ������ ó�� ���� ���� �÷��̾� ���� ����
                GameObject generatedRoom = PhotonNetwork.Instantiate("Prefabs/Map/Entrance", transform.position, transform.rotation);
                //GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //���⿡ �ִ� entrance�� ������ �������� �ɰ���
                generatedRoom.transform.SetParent(null);
                //��Ƽ�÷��� �����
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //dungeonPart.GetNetworkObject().Spawn(true); //��� Ŭ���̾�Ʈ�� ���� ���� ��ü�� �����ϴ� 
                    generatedRooms.Add(dungeonPart); //�� ����� List�� �߰�
                    SetPlayerSpawnRoom(dungeonPart);
                }
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                bool shouldPlaceHallway = false;
                if (randomValue > 0.6f) shouldPlaceHallway = true;
                //bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.6f; //������ ������ Ȯ�� 50%
                Debug.Log($"random value is {randomValue}");
                DungeonPart room1 = null; //�̹� ���� ���ο� �������� ������ ��
                Transform room1EntryPoint = null; // ������ ������ ���� �������� �� ������.
                                                  // �⺻������ ��A�� ��B�� ���� �� �� A�� ��B�� ���� �� �� �ֱ⿡ �� ��A(���� ��)�� ������(�Ա�, ��)�� ��Ÿ���� ����Ʈ

                int totalRetries = 100; //��ü ��Ʈ���� Ƚ��. ������ġ��µ�
                int retryIndex = 0;

                if (retryIndex > totalRetries) { Debug.LogError("Create room error!!"); break; }
                int ramdomGenerateRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count);
                room1 = generatedRooms[ramdomGenerateRoomIndex];
                if (room1.HasAvailableEntryPoint(out room1EntryPoint))
                {
                    DungeonPart room2 = null;
                    switch (room1EntryPoint.GetComponent<EntryPoint>().needRoomType)
                    {
                        case EntryPoint.NeedRoomType.Stair:
                            int randomStairIndex = UnityEngine.Random.Range(0, stairs.Count);
                            Debug.Log($"random value test {randomStairIndex},{stairs.Count}");
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/Multiplay/AllRooms/{stairs[randomStairIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();
                            
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); continue; }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomHallwaysIndex = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/Multiplay/AllRooms/{hallways[randomHallwaysIndex]}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); continue; }
                            break;
                        case EntryPoint.NeedRoomType.Room:
                            int randomRoomsIndex = UnityEngine.Random.Range(0, rooms.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/Multiplay/AllRooms/{rooms[randomRoomsIndex]}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); continue; }
                            break;
                        case EntryPoint.NeedRoomType.None:
                            int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                            int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/Multiplay/AllRooms/{allRoomsModules[randomRoom2Index_][randomRoom2Index__]}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); PhotonNetwork.Destroy(room2.gameObject); continue; }
                            break;
                    }
                    generatedRooms.Add(room2);
                    if (room2.dungeonPartType == DungeonPart.DungeonPartType.Hallway) { continue; }
                }
            }
        }
    }

    private void NvigationBake()
    {
        surface.BuildNavMesh();
    }

    private void SpawnRandomObject()
    {
        foreach(DungeonPart room in generatedRooms)
        {
            room.SpawnItem();
            room.SpawnMonster();
            room.SpawnObject();
        }
    }

    private void Generate()
    {
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                //���� ������ ó�� ���� ���� �÷��̾� ���� ����
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //���⿡ �ִ� entrance�� ������ �������� �ɰ���
                generatedRoom.transform.SetParent(null);
                //��Ƽ�÷��� �����
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //dungeonPart.GetNetworkObject().Spawn(true); //��� Ŭ���̾�Ʈ�� ���� ���� ��ü�� �����ϴ� 
                    generatedRooms.Add(dungeonPart); //�� ����� List�� �߰�
                    SetPlayerSpawnRoom(dungeonPart);
                }
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                bool shouldPlaceHallway = false;
                if (randomValue > 0.6f) shouldPlaceHallway = true;
                //bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.6f; //������ ������ Ȯ�� 50%
                Debug.Log($"random value is {randomValue}");
                DungeonPart room1 = null; //�̹� ���� ���ο� �������� ������ ��
                Transform room1EntryPoint = null; // ������ ������ ���� �������� �� ������.
                                                  // �⺻������ ��A�� ��B�� ���� �� �� A�� ��B�� ���� �� �� �ֱ⿡ �� ��A(���� ��)�� ������(�Ա�, ��)�� ��Ÿ���� ����Ʈ

                int totalRetries = 100; //��ü ��Ʈ���� Ƚ��. ������ġ��µ�
                int retryIndex = 0;

                if(retryIndex  > totalRetries) { Debug.LogError("Create room error!!"); break; }
                int ramdomGenerateRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count);
                room1 = generatedRooms[ramdomGenerateRoomIndex];
                if(room1.HasAvailableEntryPoint(out room1EntryPoint))
                {
                    DungeonPart room2 = null;
                    switch (room1EntryPoint.GetComponent<EntryPoint>().needRoomType)
                    {
                        case EntryPoint.NeedRoomType.Stair:
                            int randomStairIndex = UnityEngine.Random.Range(0, stairs.Count);
                            Debug.Log($"random value test {randomStairIndex},{stairs.Count}");
                            room2 = Instantiate(stairs[randomStairIndex]).GetComponent<DungeonPart>();

                            if(!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); Destroy(room2); continue; }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomHallwaysIndex = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = Instantiate(hallways[randomHallwaysIndex]).GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); Destroy(room2); continue; }
                            break;
                        case EntryPoint.NeedRoomType.Room:
                            int randomRoomsIndex = UnityEngine.Random.Range(0, rooms.Count);
                            room2 = Instantiate(rooms[randomRoomsIndex]).GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); Destroy(room2); continue; }
                            break;
                        case EntryPoint.NeedRoomType.None:
                            int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                            int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                            room2 = Instantiate(allRoomsModules[randomRoom2Index_][randomRoom2Index__]).GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint);  Destroy(room2); continue; }
                            break;
                    }
                    generatedRooms.Add(room2);
                    if(room2.dungeonPartType == DungeonPart.DungeonPartType.Hallway) { continue; }
                }

            }
        }
        Debug.Log("�Ϸ�");
    }
   




    private void RetryPlacement(GameObject itemToPlace, GameObject doorToPlace)
    {
        DungeonPart randomGeneratedRoom = null;
        Transform room1Entrypoint = null;
        int totalRetries = 100;
        int retryIndex = 0;
        while(randomGeneratedRoom == null && retryIndex < totalRetries)
        {
            int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count -1);
            DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];
            if(roomToTest.HasAvailableEntryPoint(out room1Entrypoint))
            {
                randomGeneratedRoom = roomToTest;
                break;
            }
            retryIndex++;
        }
        if(itemToPlace.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
        {
            if(dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))
            {
                doorToPlace.transform.position = room1Entrypoint.transform.position;
                doorToPlace.transform.rotation = room1Entrypoint.transform.rotation;
                AlignRooms( itemToPlace.transform, room1Entrypoint, room2Entrypoint);
                if (HandleIntersection(dungeonPart))
                {
                    dungeonPart.UnuseEntrypoint(room2Entrypoint);
                    randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint);
                    //�ϴ� �� ����
                    Debug.Log("����Լ� �� ��ħ");
                    Destroy(dungeonPart.gameObject);
                    RetryPlacement(itemToPlace, doorToPlace);
                }
            }
        }
    }
    private bool HandleIntersection(DungeonPart dungeonPart)
    {
        bool didIntersect = false;

        foreach (var collider in dungeonPart.colliderList)
        {
            Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.size / 2, Quaternion.identity, roomsLayerMask);

            foreach (var hit in hits)
            {
                if (!dungeonPart.colliderList.Contains(hit)) // �ڱ� �ڽſ� ������ ���� �ݶ��̴��� �˻�
                {
                    didIntersect = true;
                    break;
                }
            }

            if (didIntersect) break;
        }

        return didIntersect;
    }
  

    private void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry) // room1�� room2�� �Ա��� ��Ȯ�ϰ� ��ġ�ϰ� ����°� room1�� ��� ���� room1Entry�� �ʿ�
    {


        //�� �Ա��� ��ġ�� ��Ȯ�ϰ� ��ġ��Ŵ
        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;

        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.RotateAround(room2Entry.position, Vector3.up , angle);
        room2.RotateAround(room2Entry.position, Vector3.up, 180f);




        Physics.SyncTransforms();//����Ƽ���� Collider�� ����ִ� ������Ʈ�� ������ �� ����� ����ȭ�� �ȵ� ���� ����
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;
    public bool IsGenerated() => isGenerated;




    private bool AlignEntry(Transform room1EntryPoint, DungeonPart room2)
    {
        if(room2.HasAvailableEntryPoint(out Transform room2EntryPoint))
        {
            AlignRooms(room2.transform, room1EntryPoint, room2EntryPoint);
            if (HandleIntersection(room2))
            {
                room2.UnuseEntrypoint(room2EntryPoint);
            }
            else { return true; }

            foreach(Transform entryPoint in room2.entryPoints)
            {
                AlignRooms(room2.transform, room1EntryPoint, entryPoint);
                if (!HandleIntersection(room2)) { return true; }
            }
        }
        return false;
    }
    


    //TODO Player Spawn after create Room

    private void SetPlayerSpawnRoom(DungeonPart dungeonPart)
    {
        if(dungeonPart.roomUse == DungeonPart.RoomUse.PlayerSpawn)
        {
            playerSpawnDungeonPart = dungeonPart;
        }
    }

    private void PlayerSpawn()
    {
        while(playerSpawnDungeonPart == null || playerSpawnDungeonPart.dungeonPartType == DungeonPart.DungeonPartType.SpecialRoom)
        {
            playerSpawnDungeonPart = generatedRooms[UnityEngine.Random.Range(0, generatedRooms.Count)];
            if(playerSpawnDungeonPart.playerSpawnPoints.Count <= 0) { playerSpawnDungeonPart =null; }
        }
        Transform playerSpawnPosition = playerSpawnDungeonPart.playerSpawnPoints[0];
        GameObject player;
        if (SceneController.Instance.GetCurrentSceneName() == "MultiplayRoomTestScene")
        {
            player = PhotonNetwork.Instantiate("Prefabs/Player/DemoPlayer_Multiplay", playerSpawnPosition.position, Quaternion.identity);
        }
        else { player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer"), playerSpawnPosition.position, Quaternion.identity); }
        InventoryController.Instance.SetPlayer(player.transform.Find("Trigger").GetComponent<PlayerTrigger>());
        playerSpawnDungeonPart.roomUse = DungeonPart.RoomUse.PlayerSpawn;
        Debug.Log(player.transform.position);
        player_ = player;
        StartCoroutine(ResetPlayerPosition());
    }

    //���� ������ �÷��� �� �� ��� player�� position�� �������� ������ ã�� ������ ���
    GameObject player_;
    IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(1f);

        SetPlayer();
    }

    private void SetPlayer()
    {
        GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        GameObject followCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = followCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player_.transform.Find("PlayerCameraRoot");
        PlayerControl.PlayerController playerController = player_.GetComponent<PlayerControl.PlayerController>();
        playerController.SetMainCamera(mainCamera);
        InventoryController.Instance.SetPlayerInventory();
    }

    
    
    public void SpawnPlayer_Multiplay(Vector3 spawnPos)
    {
        PhotonNetwork.Instantiate($"Prefabs/Player/DemoPlayer", spawnPos, Quaternion.identity);
    }

    public Vector3 GetPlayerSpawnPosition(int index)
    {
        return playerSpawnDungeonPart.playerSpawnPoints[index+1].position;
    }

    public void StartClientAfterMapReady(int spawnRoomIndex)
    {
        surface.BuildNavMesh();

        if (generatedRooms.Count > spawnRoomIndex)
        {
            playerSpawnDungeonPart = generatedRooms[spawnRoomIndex];
        }
        else
        {
            Debug.LogWarning("���� �� �ε��� ��ȿ���� ����, ���� ����");
            playerSpawnDungeonPart = generatedRooms[UnityEngine.Random.Range(0, generatedRooms.Count)];
        }

        PlayerSpawn(); // ���� ���
    }


    //TODO player Setting


    // Update is called once per frame
    void Update()
    {
        
    }
}
