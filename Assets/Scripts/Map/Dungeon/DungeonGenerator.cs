using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Unity.AI.Navigation;


abstract public class DungeonGenerator : Singleton<DungeonGenerator>
{
    [Header("던전 구성 요소")]
    [SerializeField, Tooltip("생성될 방 개수 + 플레이어 인원")] protected int noOfRooms = 10;
    [SerializeField, Tooltip("생성될 포탈 개수_보통 플레이어 인원 + 1")] protected int potalCount = 2;
    [SerializeField] protected BossRoom bossRoom;
    [Space(10)]
    [SerializeField, Tooltip("시작지점 방")] protected GameObject entrance; //던전의 시작지점 방
    [SerializeField, Tooltip("일반적인 방")] protected List<GameObject> rooms;
    [SerializeField, Tooltip("작은 방")] protected List<GameObject> smallRooms;
    [SerializeField, Tooltip("계단")] protected List<GameObject> stairs;
    [SerializeField, Tooltip("특별한 방(던전 생성시 같은 방 하나 이상 생성 안됨. ex) 폴가이즈 식 함정방)")] protected List<GameObject> specialRooms; //전리품이 있을 수 있는 특별한 방
    [SerializeField, Tooltip("대체 입구 -> 안쓸거니 넣으면 안됨")] protected List<GameObject> alternateEntrances; //대체 입구 ex)리썰 컴퍼니의 비상구 -> 필요 없을 듯
    [SerializeField, Tooltip("복도. 생성시 방 카운트에 적용 안됨")] protected List<GameObject> hallways;
    [SerializeField, Tooltip("문. 아직 작동 안함")] protected GameObject door;

    protected List<List<GameObject>> allRoomsModules;
    //영상에선 이 오브젝트의 존재가 맵의 영향을 주지 않도록 y의 값을 -1000을 했음 아닌가? 내가 해석을 잘못했나?
    [Space(10)]
    [Header("던전매니저 구성, 바뀌면 안됨")]
    [SerializeField] protected LayerMask roomsLayerMask;
    [Space(10)]
    [Header("테스트 인스펙터")]
    [SerializeField] protected GameObject dontSelectedEntryGO;
    
    protected List<DungeonPart> generatedRooms; //던전 내부에 생성된 방List
    //private List<EntryPoint> emtryList;
    protected bool isGenerated = false;



    protected DungeonPart playerSpawnDungeonPart;
    protected DungeonPart bossDungeonPart;
    protected NetworkEventReceiver networkEventReceiver;

    [HideInInspector] public NavMeshSurface surface;
    public GameObject mainCmera;

    protected int bossCount = 1;
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
    //서버 관련
    abstract public void StartGeneration();



    //[ServerRpc(RequireOwnership = false)]
    abstract protected void StartGenerationServerRpc();
    

    protected IEnumerator GenerateMultiplay()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //아직 생성된 방이 없다면
            {
                //지금 당장은 처음 만든 방이 플레이어 스폰 방임
                Debug.Log("Photon test");
                GameObject generatedRoom = PhotonNetwork.Instantiate("Prefabs/Map/Entrance", transform.position, transform.rotation);
                //GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //여기에 있는 entrance가 던전의 시작점이 될것임
                generatedRoom.transform.SetParent(null);
                //멀티플레이 요소임
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //dungeonPart.GetNetworkObject().Spawn(true); //모든 클라이언트에 대한 게임 객체를 생성하는 
                    generatedRooms.Add(dungeonPart); //방 만든거 List에 추가
                }
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                //bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.6f; //복도를 생성할 확률 50%
                DungeonPart room1 = null; //이미 던전 내부에 무작위로 생성된 방
                Transform room1EntryPoint = null; // 이전에 생성된 방의 진입점이 될 변형점.
                                                  // 기본적으로 방A와 방B가 있을 때 방 A와 방B가 연결 될 수 있기에 그 방A(이전 방)의 진입점(입구, 문)을 나타내는 포인트

                int totalRetries = 100; //전체 리트라이 횟수. 안전장치라는데
                int retryIndex = 0;

                if (retryIndex > totalRetries) { Debug.LogError("Create room error!!"); break; }
                int ramdomGenerateRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count);
                room1 = generatedRooms[ramdomGenerateRoomIndex];
                if (room1.HasAvailableEntryPoint(out room1EntryPoint))
                {
                    DungeonPart room2 = null;
                    bool aligned = false;
                    switch (room1EntryPoint.GetComponent<EntryPoint>().needRoomType)
                    {
                        case EntryPoint.NeedRoomType.Stair:
                            int randomStairIndex = UnityEngine.Random.Range(0, stairs.Count);
                            Debug.Log($"random value test {randomStairIndex},{stairs.Count}");
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Stairs/{stairs[randomStairIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            
                            yield return StartCoroutine(AlignEntryCoroutine(room1EntryPoint, room2, result => aligned = result));
                            if (!aligned)
                            {
                                room1.UnuseEntrypoint(room1EntryPoint);
                                PhotonNetwork.Destroy(room2.gameObject);
                                continue;
                            }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomHallwaysIndex = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Hallways/{hallways[randomHallwaysIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            
                            yield return StartCoroutine(AlignEntryCoroutine(room1EntryPoint, room2, result => aligned = result));
                            if (!aligned)
                            {
                                room1.UnuseEntrypoint(room1EntryPoint);
                                PhotonNetwork.Destroy(room2.gameObject);
                                continue;
                            }
                            break;
                        case EntryPoint.NeedRoomType.Room:
                            int randomRoomsIndex = UnityEngine.Random.Range(0, rooms.Count);
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/Rooms/{rooms[randomRoomsIndex].name}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            
                            yield return StartCoroutine(AlignEntryCoroutine(room1EntryPoint, room2, result => aligned = result));
                            if (!aligned)
                            {
                                room1.UnuseEntrypoint(room1EntryPoint);
                                PhotonNetwork.Destroy(room2.gameObject);
                                continue;
                            }
                            break;
                        case EntryPoint.NeedRoomType.None:
                            int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                            int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                            Debug.Log("None test");
                            room2 = PhotonNetwork.Instantiate($"Prefabs/Map/MultiPlay/AllRooms/{allRoomsModules[randomRoom2Index_][randomRoom2Index__]}"
                                , transform.position, transform.rotation).GetComponent<DungeonPart>();

                            
                            yield return StartCoroutine(AlignEntryCoroutine(room1EntryPoint, room2, result => aligned = result));
                            if (!aligned)
                            {
                                room1.UnuseEntrypoint(room1EntryPoint);
                                PhotonNetwork.Destroy(room2.gameObject);
                                continue;
                            }
                            break;
                    }
                    generatedRooms.Add(room2);
                    if (room2.dungeonPartType == DungeonPart.DungeonPartType.Hallway) { continue; }
                }
            }
        }
    }



    protected void NvigationBake()
    {
        surface.BuildNavMesh();
    }

    protected void SpawnRandomObject()
    {
        foreach(DungeonPart room in generatedRooms)
        {
            room.SpawnItem();
            room.SpawnMonster();
            room.SpawnObject();
        }
    }

    protected void Generate()
    {
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //아직 생성된 방이 없다면
            {
                //지금 당장은 처음 만든 방이 플레이어 스폰 방임
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //여기에 있는 entrance가 던전의 시작점이 될것임
                generatedRoom.transform.SetParent(null);
                //멀티플레이 요소임
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //dungeonPart.GetNetworkObject().Spawn(true); //모든 클라이언트에 대한 게임 객체를 생성하는 
                    generatedRooms.Add(dungeonPart); //방 만든거 List에 추가
                    SetPlayerSpawnRoom(dungeonPart);
                }
            }
            else
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                bool shouldPlaceHallway = false;
                if (randomValue > 0.6f) shouldPlaceHallway = true;
                //bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.6f; //복도를 생성할 확률 50%
                Debug.Log($"random value is {randomValue}");
                DungeonPart room1 = null; //이미 던전 내부에 무작위로 생성된 방
                Transform room1EntryPoint = null; // 이전에 생성된 방의 진입점이 될 변형점.
                                                  // 기본적으로 방A와 방B가 있을 때 방 A와 방B가 연결 될 수 있기에 그 방A(이전 방)의 진입점(입구, 문)을 나타내는 포인트

                int totalRetries = 100; //전체 리트라이 횟수. 안전장치라는데
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
        Debug.Log("완료");
        isGenerated = true;
    }





    protected void RetryPlacement(GameObject itemToPlace, GameObject doorToPlace)
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
                    //일단 방 지움
                    Debug.Log("재귀함수 방 곂침");
                    Destroy(dungeonPart.gameObject);
                    RetryPlacement(itemToPlace, doorToPlace);
                }
            }
        }
    }
    protected bool HandleIntersection(DungeonPart dungeonPart)
    {
        bool didIntersect = false;

        foreach (var collider in dungeonPart.colliderList)
        {
            Physics.SyncTransforms();
            Collider[] hits = Physics.OverlapBox(collider.bounds.center, collider.bounds.size / 2, Quaternion.identity, roomsLayerMask);

            foreach (var hit in hits)
            {
                if (!dungeonPart.colliderList.Contains(hit)) // 자기 자신에 속하지 않은 콜라이더만 검사
                {
                    
                    didIntersect = true;
                    break;
                }
                Debug.Log("collider is ok");
            }

            if (didIntersect) break;
        }

        return didIntersect;
    }


    protected void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry) // room1과 room2의 입구가 정확하게 일치하게 만드는것 room1은 사용 안함 room1Entry만 필요
    {


        //두 입구의 위치를 정확하게 일치시킴
        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;

        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.RotateAround(room2Entry.position, Vector3.up , angle);
        room2.RotateAround(room2Entry.position, Vector3.up, 180f);




        Physics.SyncTransforms();//유니티에서 Collider가 들어있는 오브젝트가 움직일 때 제대로 동기화가 안될 때가 있음
    }
    protected IEnumerator AlignEntryCoroutine(Transform room1EntryPoint, DungeonPart room2, Action<bool> callback)
    {
        if (room2.HasAvailableEntryPoint(out Transform room2EntryPoint))
        {
            AlignRooms(room2.transform, room1EntryPoint, room2EntryPoint);
            yield return new WaitForSeconds(1f); // 정렬 직후 대기

            if (HandleIntersection(room2))
            {
                room2.UnuseEntrypoint(room2EntryPoint);
            }
            else
            {
                
                callback(true); yield break;
            }

            foreach (Transform entryPoint in room2.entryPoints)
            {
                AlignRooms(room2.transform, room1EntryPoint, entryPoint);
                yield return new WaitForSeconds(1f); // 다른 포인트 정렬 후 대기

                if (!HandleIntersection(room2))
                {
                    callback(true); yield break;
                }
            }
        }

        callback(false);
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;
    public bool IsGenerated() => isGenerated;




    protected bool AlignEntry(Transform room1EntryPoint, DungeonPart room2)
    {
        if(room2.dungeonPartType == DungeonPart.DungeonPartType.SpecialRoom)
        {
            if (room2.HasAvailableEntryPoint(out Transform room2EntryPoint))
            {
                AlignRooms(room2.transform, room1EntryPoint, room2EntryPoint);
                if (HandleIntersection(room2))
                {
                    room2.UnuseEntrypoint(room2EntryPoint);
                }
                else { return true; }

                foreach (Transform entryPoint in room2.entryPoints)
                {
                    AlignRooms(room2.transform, room1EntryPoint, entryPoint);
                    if (!HandleIntersection(room2)) { return true; }
                }
            }
        }
        else
        {
            if(room2.HasAvailableEntryPointTrapRoom2(out Transform room2EntryPoint)){
                AlignRooms(room2.transform, room1EntryPoint, room2EntryPoint);
                if (HandleIntersection(room2))
                {
                    room2.UnuseEntrypoint(room2EntryPoint);
                }
                else { return true; }
            }
        }
        
        return false;
    }



    //TODO Player Spawn after create Room

    protected void SetPlayerSpawnRoom(DungeonPart dungeonPart)
    {
        if(dungeonPart.roomUse == DungeonPart.RoomUse.PlayerSpawn)
        {
            playerSpawnDungeonPart = dungeonPart;
        }
    }

    abstract protected void PlayerSpawn();


    //지금 게임을 플레이 할 때 계속 player의 position을 돌려버림 왜인지 찾을 때까지 대기
    protected GameObject player_;
    protected IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(1f);

        SetPlayer();
    }

    abstract protected IEnumerator WaitOtherPlayerEnter(GameObject player);
 

    protected IEnumerator WaitHostReady()
    {
        while (!networkEventReceiver.playerSpawnReady)
        {
            Debug.Log("Wait host ready");
            yield return new WaitForSeconds(1f);
        }
        NvigationBake();
        InventoryController.Instance.SetPlayer(player_.transform.Find("Trigger").GetComponent<PlayerTrigger>());
        InventoryController.Instance.SetPlayerInventory();
    }





    protected void SetPlayer()
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
        GameObject player = PhotonNetwork.Instantiate($"Prefabs/Player/Player_Multiplay", spawnPos, Quaternion.identity);
        GameObject mainCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/MainCamera"));
        GameObject followCamera = Instantiate(Resources.Load<GameObject>($"Prefabs/Camera/PlayerFollowCamera"));
        CinemachineVirtualCamera virtualCamera = followCamera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform.Find("PlayerCameraRoot");
        PlayerControl.PlayerController playerController = player.GetComponent<PlayerControl.PlayerController>();
        playerController.SetMainCamera(mainCamera);
        mainCmera = mainCamera;
        networkEventReceiver.SendThisPlayerReady();
        player_ = player;
    }

    public Vector3 GetPlayerSpawnPosition(int index)
    {
        return playerSpawnDungeonPart.playerSpawnPoints[index-1].position;
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
            Debug.LogWarning("스폰 룸 인덱스 유효하지 않음, 임의 선택");
            playerSpawnDungeonPart = generatedRooms[UnityEngine.Random.Range(0, generatedRooms.Count)];
        }

        PlayerSpawn(); // 기존 방식
    }

    public void ClientPlayerSpawn()
    {
        PlayerSpawn();
        NvigationBake();
    }


    //TODO player Setting


    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void FillWall()
    {

    }

    public virtual void EnterBossRoom()
    {
        Debug.Log($"potal test 2 {InventoryController.Instance.playerController.transform.position}");
        InventoryController.Instance.playerController.transform.position = bossRoom.playerSpawnPoint.position;
        Debug.Log($"potal test 3 {InventoryController.Instance.playerController.transform.position}");
    }

    protected virtual void SetEscapePotal()
    {

    }

    public void CountBossKill()
    {
        bossCount -= 1;
        if (bossCount <= 0)
        {
            bossRoom.SpawnEscapePotal();
        }
    }

}
