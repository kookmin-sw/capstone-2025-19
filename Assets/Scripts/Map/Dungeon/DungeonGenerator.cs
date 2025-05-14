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
    [Header("���� ���� ���")]
    [SerializeField, Tooltip("������ �� ���� + �÷��̾� �ο�")] protected int noOfRooms = 10;
    [SerializeField, Tooltip("������ ��Ż ����_���� �÷��̾� �ο� + 1")] protected int potalCount = 2;
    [SerializeField] protected BossRoom bossRoom;
    [Space(10)]
    [SerializeField, Tooltip("�������� ��")] protected GameObject entrance; //������ �������� ��
    [SerializeField, Tooltip("�Ϲ����� ��")] protected List<GameObject> rooms;
    [SerializeField, Tooltip("���� ��")] protected List<GameObject> smallRooms;
    [SerializeField, Tooltip("���")] protected List<GameObject> stairs;
    [SerializeField, Tooltip("Ư���� ��(���� ������ ���� �� �ϳ� �̻� ���� �ȵ�. ex) �������� �� ������)")] protected List<GameObject> specialRooms; //����ǰ�� ���� �� �ִ� Ư���� ��
    [SerializeField, Tooltip("��ü �Ա� -> �Ⱦ��Ŵ� ������ �ȵ�")] protected List<GameObject> alternateEntrances; //��ü �Ա� ex)���� ���۴��� ��� -> �ʿ� ���� ��
    [SerializeField, Tooltip("����. ������ �� ī��Ʈ�� ���� �ȵ�")] protected List<GameObject> hallways;
    [SerializeField, Tooltip("��. ���� �۵� ����")] protected GameObject door;

    protected List<List<GameObject>> allRoomsModules;
    //���󿡼� �� ������Ʈ�� ���簡 ���� ������ ���� �ʵ��� y�� ���� -1000�� ���� �ƴѰ�? ���� �ؼ��� �߸��߳�?
    [Space(10)]
    [Header("�����Ŵ��� ����, �ٲ�� �ȵ�")]
    [SerializeField] protected LayerMask roomsLayerMask;
    [Space(10)]
    [Header("�׽�Ʈ �ν�����")]
    [SerializeField] protected GameObject dontSelectedEntryGO;
    
    protected List<DungeonPart> generatedRooms; //���� ���ο� ������ ��List
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
    //���� ����
    abstract public void StartGeneration();



    //[ServerRpc(RequireOwnership = false)]
    abstract protected void StartGenerationServerRpc();
    

    protected IEnumerator GenerateMultiplay()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                //���� ������ ó�� ���� ���� �÷��̾� ���� ����
                Debug.Log("Photon test");
                GameObject generatedRoom = PhotonNetwork.Instantiate("Prefabs/Map/Entrance", transform.position, transform.rotation);
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
                    //�ϴ� �� ����
                    Debug.Log("����Լ� �� ��ħ");
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
                if (!dungeonPart.colliderList.Contains(hit)) // �ڱ� �ڽſ� ������ ���� �ݶ��̴��� �˻�
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


    protected void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry) // room1�� room2�� �Ա��� ��Ȯ�ϰ� ��ġ�ϰ� ����°� room1�� ��� ���� room1Entry�� �ʿ�
    {


        //�� �Ա��� ��ġ�� ��Ȯ�ϰ� ��ġ��Ŵ
        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;

        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.RotateAround(room2Entry.position, Vector3.up , angle);
        room2.RotateAround(room2Entry.position, Vector3.up, 180f);




        Physics.SyncTransforms();//����Ƽ���� Collider�� ����ִ� ������Ʈ�� ������ �� ����� ����ȭ�� �ȵ� ���� ����
    }
    protected IEnumerator AlignEntryCoroutine(Transform room1EntryPoint, DungeonPart room2, Action<bool> callback)
    {
        if (room2.HasAvailableEntryPoint(out Transform room2EntryPoint))
        {
            AlignRooms(room2.transform, room1EntryPoint, room2EntryPoint);
            yield return new WaitForSeconds(1f); // ���� ���� ���

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
                yield return new WaitForSeconds(1f); // �ٸ� ����Ʈ ���� �� ���

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


    //���� ������ �÷��� �� �� ��� player�� position�� �������� ������ ã�� ������ ���
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
            Debug.LogWarning("���� �� �ε��� ��ȿ���� ����, ���� ����");
            playerSpawnDungeonPart = generatedRooms[UnityEngine.Random.Range(0, generatedRooms.Count)];
        }

        PlayerSpawn(); // ���� ���
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
