using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class DungeonGenerator : Singleton<DungeonGenerator>
{
    [Header("던전 구성 요소")]
    [SerializeField, Tooltip("생성될 방 개수")] private int noOfRooms = 10;
    [Space(10)]
    [SerializeField, Tooltip("시작지점 방")] private GameObject entrance; //던전의 시작지점 방
    [SerializeField, Tooltip("일반적인 방")] private List<GameObject> rooms;
    [SerializeField, Tooltip("계단")] private List<GameObject> stairs;
    [SerializeField, Tooltip("특별한 방(던전 생성시 같은 방 하나 이상 생성 안됨. ex) 폴가이즈 식 함정방)")] private List<GameObject> specialRooms; //전리품이 있을 수 있는 특별한 방
    [SerializeField, Tooltip("대체 입구 -> 안쓸거니 넣으면 안됨")] private List<GameObject> alternateEntrances; //대체 입구 ex)리썰 컴퍼니의 비상구 -> 필요 없을 듯
    [SerializeField, Tooltip("복도. 생성시 방 카운트에 적용 안됨")] private List<GameObject> hallways;
    [SerializeField, Tooltip("문. 아직 작동 안함")] private GameObject door;

    private List<List<GameObject>> allRoomsModules;
    //영상에선 이 오브젝트의 존재가 맵의 영향을 주지 않도록 y의 값을 -1000을 했음 아닌가? 내가 해석을 잘못했나?
    [Space(10)]
    [Header("던전매니저 구성, 바뀌면 안됨")]
    [SerializeField] LayerMask roomsLayerMask;
    [Space(10)]
    [Header("테스트 인스펙터")]
    [SerializeField] GameObject dontSelectedEntryGO;
    
    private List<DungeonPart> generatedRooms; //던전 내부에 생성된 방List
    //private List<EntryPoint> emtryList;
    private bool isGenerated = false;


    private DungeonPart playerSpawnDungeonPart;

    [HideInInspector] public NavMeshSurface surface;


    // Start is called before the first frame update
    void Start()
    {
        //Add room list;
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
    public void StartGeneration()
    {
        StartGenerationServerRpc();
    }

    //[ServerRpc(RequireOwnership = false)]
    private void StartGenerationServerRpc()
    {
        Generate(); //던전 생성
        //GenerateAlternateEntrances(); //다른 입구 생성 -> 일단 보류. 알고리즘 자체는 던전 방 생성과 똑같음
        //FillEmptyEntrances(); //모든 방이 생성된 이후 남은 입구 벽으로 막기

        NvigationBake();
        SpawnRandomObject();
        PlayerSpawn();
        isGenerated = true;
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
                            int randommRoom2Index = UnityEngine.Random.Range(0, stairs.Count);
                            room2 = stairs[randommRoom2Index].GetComponent<DungeonPart>();
                            if(!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); continue; }
                            break;
                        case EntryPoint.NeedRoomType.Hallway:
                            int randomRoom2Index = UnityEngine.Random.Range(0, hallways.Count);
                            room2 = hallways[randomRoom2Index].GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); continue; }
                            break;
                        case EntryPoint.NeedRoomType.None:
                            int randomRoom2Index_ = UnityEngine.Random.Range(0, allRoomsModules.Count);
                            int randomRoom2Index__ = UnityEngine.Random.Range(0, allRoomsModules[randomRoom2Index_].Count);
                            room2 = allRoomsModules[randomRoom2Index_][randomRoom2Index__].GetComponent<DungeonPart>();
                            if (!AlignEntry(room1EntryPoint, room2)) { room1.UnuseEntrypoint(room1EntryPoint); continue; }
                            break;
                    }
                    generatedRooms.Add(room2);
                    if(room2.dungeonPartType == DungeonPart.DungeonPartType.Hallway) { continue; }
                }
                

                /*if (shouldPlaceHallway)//50% 확률로 복도를 설치해야 할 경우 복도를 방 위주로 설치 할건지 
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject room2 = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//복도 생성
                    room2.transform.SetParent(null);
                    if (room2.TryGetComponent<DungeonPart>(out DungeonPart room2_dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // 멀티플레이 요소
                        if (room2_dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//생성한 복도에 랜덤한 입구 고르기
                        {
                            while (room1 == null && retryIndex < totalRetries) // 무작위 행동 while문에 대해 혹시 모르는 무한 루프에 빠질 가능성을 지우기 위해 최대 100번까지만 루프 돌게 함
                            {
                                int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //임의의 방을 선택
                                DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                                if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//그 선택한 방에서 입구를 찾고 있으면 room1EntryPoint에 할당하고 
                                {
                                    room1 = roomToTest;//randomGeneratedRoom에 선택한 방을 할당 후 while문 break
                                    break;
                                }
                                retryIndex++; //안전장치
                            }
                            // 생성한 방에서 연결할 방 고르기
                            *//*foreach(DungeonPart dungeonPart in generatedRooms)
                            {

                            }
                            if(room1.HasAvailableEntryPoint(out room1EntryPoint))*//*


                            AlignRooms(room1.transform, room2.transform, room1EntryPoint, room2Entrypoint); //방 정렬 (첫번째 방, 지금 생성한 복도,
                                                                                                            //첫번째 방 입구, 지금 생성한 복도 입구)

                            //if (HandleIntersection(room2_dungeonPart)) //위치가 미리 생성한 방이랑 곂친다던가 하는 문제가 있을 시 되돌리는 부분인거 같은데
                            bool innerTest = false;
                            if (room2_dungeonPart.IsColliderOverlapping())
                            {
                                room2_dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                room1.UnuseEntrypoint(room1EntryPoint);
                                //그래서 생성한 방 언제 지움?
                                //일단 지워봄
                                Debug.Log("복도 곂침");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //일단 재귀함수로 새로운 방, 새로운 입구 찾기
                                //TODO 그냥모든 방, 입구를 하나씩 대조해 보면서 넣기
                                bool finalCheck = false;
                                foreach (DungeonPart dungeon in generatedRooms)
                                {
                                    bool check1 = false;

                                    foreach (Transform room1Transform in dungeon.GetEntryPointList())
                                    {
                                        bool check2 = false;
                                        foreach (Transform room2Transform in room2.GetComponent<DungeonPart>().GetEntryPointList())
                                        {
                                            AlignRooms(dungeon.transform, room2.transform, room1Transform, room2Transform);

                                            if (!room2_dungeonPart.IsColliderOverlapping())
                                            {
                                                generatedRooms.Add(room2_dungeonPart);//생성한 복도에서 알맞은 입구를 골랐으면 생성한 복도를 generatedRooms(생성한 방 리스트)에 추가
                                                Debug.LogError("화면 집중");
                                                room1Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                room2Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                check1 = true;
                                                check2 = true;
                                                finalCheck = true;
                                                innerTest = true;
                                                Debug.Log("통과");
                                                break;
                                            }
                                            Debug.Log("room2(Hallway)의 입구 바꾸기");
                                        }
                                        if (check2) break;
                                        Debug.Log("room1의 입구 바꾸기");
                                    }
                                    if (check1) break;
                                    Debug.Log("room1을 바꾸기");
                                }
                                if (!finalCheck)
                                {
                                    Debug.Log("room2(Hallway)를 바꾸기 !!! ");
                                    Destroy(room2_dungeonPart.gameObject);
                                    continue;
                                }

                            }
                            if (!innerTest)
                            {
                                room1EntryPoint.GetComponent<EntryPoint>().SetOccupied(true);
                                room2Entrypoint.GetComponent<EntryPoint>().SetOccupied(true);
                                generatedRooms.Add(room2_dungeonPart);
                            }
                            i -= 1;
                            continue;
                        }
                    }
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, rooms.Count);
                    GameObject room2 = Instantiate(rooms[randomIndex], transform.position, transform.rotation);//복도 생성
                    room2.transform.SetParent(null);
                    if (room2.TryGetComponent<DungeonPart>(out DungeonPart room2_dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // 멀티플레이 요소
                        if (room2_dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//생성한 복도에 랜덤한 입구 고르기
                        {
                            while (room1 == null && retryIndex < totalRetries) // 무작위 행동 while문에 대해 혹시 모르는 무한 루프에 빠질 가능성을 지우기 위해 최대 100번까지만 루프 돌게 함
                            {
                                int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //임의의 방을 선택
                                DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                                if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//그 선택한 방에서 입구를 찾고 있으면 room1EntryPoint에 할당하고 
                                {
                                    room1 = roomToTest;//randomGeneratedRoom에 선택한 방을 할당 후 while문 break
                                    break;
                                }
                                retryIndex++; //안전장치
                            }
                            // 생성한 방에서 연결할 방 고르기
                            *//*foreach(DungeonPart dungeonPart in generatedRooms)
                            {

                            }
                            if(room1.HasAvailableEntryPoint(out room1EntryPoint))*//*


                            AlignRooms(room1.transform, room2.transform, room1EntryPoint, room2Entrypoint); //방 정렬 (첫번째 방, 지금 생성한 복도,
                                                                                                            //첫번째 방 입구, 지금 생성한 복도 입구)
                            Debug.Log("test");
                            bool innerTest = false;
                            if (room2_dungeonPart.IsColliderOverlapping())
                            //if (HandleIntersection(room2_dungeonPart)) //위치가 미리 생성한 방이랑 곂친다던가 하는 문제가 있을 시 되돌리는 부분인거 같은데
                            {
                                room2_dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                room1.UnuseEntrypoint(room1EntryPoint);
                                //그래서 생성한 방 언제 지움?
                                //일단 지워봄
                                Debug.Log("복도 곂침");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //일단 재귀함수로 새로운 방, 새로운 입구 찾기
                                //TODO 그냥모든 방, 입구를 하나씩 대조해 보면서 넣기
                                bool finalCheck = false;
                                foreach (DungeonPart dungeon in generatedRooms)
                                {
                                    bool check1 = false;

                                    foreach (Transform room1Transform in dungeon.GetEntryPointList())
                                    {
                                        bool check2 = false;
                                        foreach (Transform room2Transform in room2.GetComponent<DungeonPart>().GetEntryPointList())
                                        {
                                            AlignRooms(dungeon.transform, room2.transform, room1Transform, room2Transform);

                                            if (!room2_dungeonPart.IsColliderOverlapping())
                                            {
                                                generatedRooms.Add(room2_dungeonPart);//생성한 복도에서 알맞은 입구를 골랐으면 생성한 복도를 generatedRooms(생성한 방 리스트)에 추가
                                                Debug.LogError("화면 집중");
                                                room1Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                //room2Entrypoint
                                                room2Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                check1 = true;
                                                check2 = true;
                                                finalCheck = true;
                                                innerTest = true;
                                                Debug.Log("통과");
                                                break;
                                            }
                                            Debug.Log("room2(Hallway)의 입구 바꾸기");
                                        }
                                        if (check2) break;
                                        Debug.Log("room1의 입구 바꾸기");
                                    }
                                    if (check1) break;
                                    Debug.Log("room1을 바꾸기");
                                }
                                if (!finalCheck)
                                {
                                    Debug.LogError("room2(room)를 바꾸기 !!! ");
                                    Destroy(room2_dungeonPart.gameObject);
                                    continue;
                                }

                            }
                            if (!innerTest)
                            {
                                generatedRooms.Add(room2_dungeonPart);
                                room1EntryPoint.GetComponent<EntryPoint>().SetOccupied(true);
                                room2Entrypoint.GetComponent<EntryPoint>().SetOccupied(true);
                            }
                        }
                    }
                }*/
            }
        }
        Debug.Log("완료");
    }
    /*private void TestGenerate() //일단 내가 원하는 방의 개수를 넘어서 방을 만들고 싶지 않음 -> 그럼 이렇게 되면 미리 만들어 놓은 방을 다 쓰되 그 이상은 없나?
    {
        for(int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if(generatedRooms.Count < 1) //아직 생성된 방이 없다면
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //여기에 있는 entrance가 던전의 시작점이 될것임
                generatedRoom.transform.SetParent(null);// 생성된 방이 어떤 parent에 속해 있다면 Position 값이 parent에게 영향 받을 수 있기에 혹시나 parent 설정되어있으면 null로 할당하기
                
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //멀티플레이 요소임
                    //dungeonPart.GetNetworkObject().Spawn(true); //모든 클라이언트에 대한 게임 객체를 생성하는 -> photonView.Instantiate()형식
                    generatedRooms.Add(dungeonPart); //방 만든거 List에 추가
                }
            }
            else
            {
                bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.5f; //복도를 생성할 확률 50%
                DungeonPart randomGeneratedRoom = null; //이미 던전 내부에 무작위로 생성된 방
                Transform room1EntryPoint = null; // 이전에 생성된 방의 진입점이 될 변형점.
                                                  // 기본적으로 방A와 방B가 있을 때 방 A와 방B가 연결 될 수 있기에 그 방A(이전 방)의 진입점(입구, 문)을 나타내는 포인트

                int totalRetries = 100; //전체 리트라이 횟수. 안전장치라는데
                int retryIndex = 0;


                while (randomGeneratedRoom == null && retryIndex < totalRetries) // 무작위 행동 while문에 대해 혹시 모르는 무한 루프에 빠질 가능성을 지우기 위해 최대 100번까지만 루프 돌게 함
                {
                    int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //임의의 생성된 방을 선택
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                    if(roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//그 선택한 방에서 입구를 찾고 있으면 room1EntryPoint에 할당하고 
                    {
                        randomGeneratedRoom = roomToTest;//randomGeneratedRoom에 선택한 방을 할당 후 while문 break
                        break;
                    }
                    retryIndex++; //안전장치
                }
                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
                //doorToAlign.GetComponent<NetworkObject>().Spawn(true); // 멀티플레이요소

                if (shouldPlaceHallway)//50%확률로 복도를 설치해야 할 경우
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//복도 생성
                    generatedHallway.transform.SetParent(null);
                    if(generatedHallway.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // 멀티플레이 요소
                        if(dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//생성한 복도에 랜덤한 입구 고르기
                        {
                            generatedRooms.Add(dungeonPart);//생성한 복도에서 알맞은 입구를 골랐으면 생성한 복도를 generatedRooms(생성한 방 리스트)에 추가
                            doorToAlign.transform.position = room1EntryPoint.transform.position;//맞는 위치에 맞추기
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2Entrypoint); //방 정렬 (첫번째 방, 지금 생성한 복도,
                                                                                                                                     //첫번째 방 입구, 지금 생성한 복도 입구)
                            if (HandleIntersection(dungeonPart)) //위치가 미리 생성한 방이랑 곂친다던가 하는 문제가 있을 시 되돌리는 부분인거 같은데
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                //그래서 생성한 방 언제 지움?
                                //일단 지워봄
                                Debug.Log("복도 곂침");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //일단 재귀함수로 새로운 방, 새로운 입구 찾기
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;
                    if(specialRooms.Count > 0) // 특별한 방이 만들 생각이 있을 경우
                    {
                        bool shouldPlaceSpecialRoom = UnityEngine.Random.Range(0f, 1f) > 0.9f;//10프로 확률로 특별한 방이 생성됨 -> 난 그냥 하나씩 만들까 할듯
                        if(shouldPlaceSpecialRoom)
                        {
                            int randomIndex = UnityEngine.Random.Range(0, specialRooms.Count); //특별한 방 만들기
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = UnityEngine.Random.Range(0, rooms.Count); //평범한 방 만들기
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else // 아닐 경우 (특별한 방 만들 생각 없음)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                    }

                    generatedRoom.transform.SetParent(null);
                    if(generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))//생성한 방 위치 설정
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); //멀티플레이 요소
                        if(dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2Entrypoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                //일단 방 곂치는 방 지워봄
                                Debug.Log("방 곂침");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedRoom, doorToAlign);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }*/

    
    /*private void GenerateAlternateEntrances()
    {
        if (alternateEntrances.Count < 1) return;
        for (int i = 0; i < alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1) //아직 생성된 방이 없다면
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //여기에 있는 entrance가 던전의 시작점이 될것임
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
                bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.5f; //복도를 생성할 확률 50%
                DungeonPart randomGeneratedRoom = null; //이미 던전 내부에 무작위로 생성된 방
                Transform room1EntryPoint = null; // 이전에 생성된 방의 진입점이 될 변형점.
                                                  // 기본적으로 방A와 방B가 있을 때 방 A와 방B가 연결 될 수 있기에 그 방A(이전 방)의 진입점(입구, 문)을 나타내는 포인트

                int totalRetries = 100; //전체 리트라이 횟수. 안전장치라는데
                int retryIndex = 0;


                while (randomGeneratedRoom == null && retryIndex < totalRetries) // 무작위 행동 while문에 대해 혹시 모르는 무한 루프에 빠질 가능성을 지우기 위해 최대 100번까지만 루프 돌게 함
                {
                    int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //임의의 방을 선택
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                    if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//그 선택한 방에서 입구를 찾고 있으면 room1EntryPoint에 할당하고 
                    {
                        randomGeneratedRoom = roomToTest;//randomGeneratedRoom에 선택한 방을 할당 후 while문 break
                        break;
                    }
                    retryIndex++; //안전장치
                }
                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
                //doorToAlign.GetComponent<NetworkObject>().Spawn(true); // 멀티플레이요소

                if (shouldPlaceHallway)//50%확률로 복도를 설치해야 할 경우
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//복도 생성
                    generatedHallway.transform.SetParent(null);
                    if (generatedHallway.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // 멀티플레이 요소
                        if (dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//생성한 복도에 랜덤한 입구 고르기
                        {
                            generatedRooms.Add(dungeonPart);//생성한 복도에서 알맞은 입구를 골랐으면 생성한 복도를 generatedRooms(생성한 방 리스트)에 추가
                            doorToAlign.transform.position = room1EntryPoint.transform.position;//맞는 위치에 맞추기
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2Entrypoint); //방 정렬 (첫번째 방, 지금 생성한 복도,
                                                                                                                                     //첫번째 방 입구, 지금 생성한 복도 입구)
                            if (HandleIntersection(dungeonPart)) //위치가 미리 생성한 방이랑 곂친다던가 하는 문제가 있을 시 되돌리는 부분인거 같은데
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                //그래서 생성한 방 언제 지움?
                                RetryPlacement(generatedHallway, doorToAlign); //일단 재귀함수로 새로운 방, 새로운 입구 찾기
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;
                    if (specialRooms.Count > 0) // 특별한 방이 만들 생각이 있을 경우
                    {
                        bool shouldPlaceSpecialRoom = UnityEngine.Random.Range(0f, 1f) > 0.9f;//10프로 확률로 특별한 방이 생성됨 -> 난 그냥 하나씩 만들까 할듯
                        if (shouldPlaceSpecialRoom)
                        {
                            int randomIndex = UnityEngine.Random.Range(0, specialRooms.Count); //특별한 방 만들기
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = UnityEngine.Random.Range(0, rooms.Count); //평범한 방 만들기
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else // 아닐 경우 (특별한 방 만들 생각 없음)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                    }

                    generatedRoom.transform.SetParent(null);
                    if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))//생성한 방 위치 설정
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); //멀티플레이 요소
                        if (dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2Entrypoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                RetryPlacement(generatedRoom, doorToAlign);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }*/

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
                    //일단 방 지움
                    Debug.Log("재귀함수 방 곂침");
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
                if (!dungeonPart.colliderList.Contains(hit)) // 자기 자신에 속하지 않은 콜라이더만 검사
                {
                    didIntersect = true;
                    break;
                }
            }

            if (didIntersect) break;
        }

        return didIntersect;
    }
    //기존 코드
    /*private bool HandleIntersection(DungeonPart dungeonPart)// 생성한 방(혹은 복도)가 서로 곂치는지 확인
    {
        bool didIntersect = false;
        Collider[] hits = Physics.OverlapBox(dungeonPart.collider.bounds.center, dungeonPart.collider.bounds.size / 2, Quaternion.identity, roomsLayerMask);

        foreach(Collider hit in hits)
        {
            *//*if (hit == dungeonPart.collider) continue;
            if(hit != dungeonPart.collider)
            {
                didIntersect = true;
                break;
            }*//*
            if (hit != dungeonPart.collider) // 자기 자신과 충돌이 아닌 경우만 검사
            {
                didIntersect = true;
                break;
            }
        }
        return didIntersect;
    }*/

    private void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry) // room1과 room2의 입구가 정확하게 일치하게 만드는것 room1은 사용 안함 room1Entry만 필요
    {
        //영상에 나온 코드
        /*float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);

        room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + angle, room2.eulerAngles.z);
        Vector3 offset = room1Entry.position - room2Entry.position;

        room2.position += offset;*/

        //두 입구의 위치를 정확하게 일치시킴
        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;

        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.RotateAround(room2Entry.position, Vector3.up , angle);
        room2.RotateAround(room2Entry.position, Vector3.up, 180f);

        /*float angle = Vector3.SignedAngle(room2Entry.forward, -room1Entry.forward, Vector3.up);
        room2.Rotate(0, angle, 0, Space.World);*/


        Physics.SyncTransforms();//유니티에서 Collider가 들어있는 오브젝트가 움직일 때 제대로 동기화가 안될 때가 있음
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;
    public bool IsGenerated() => isGenerated;

    private void FillEmptyEntrances()
    {
        generatedRooms.ForEach(room => room.FillEmptyDoors());
    }


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
        //지금 당장은 PhotonView를 쓰는 것이 아님
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer"));
        player.transform.localPosition = playerSpawnDungeonPart.spawnPoint.position;
        InventoryController.Instance.SetPlayer(player.transform.Find("Trigger").GetComponent<PlayerTrigger>());
        Debug.Log(player.transform.position);
        player_ = player;
        StartCoroutine(ResetPlayerPosition());
    }

    //지금 게임을 플레이 할 때 계속 player의 position을 돌려버림 왜인지 찾을 때까지 대기
    GameObject player_;
    IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(1f);
        Debug.LogError($"player position {player_.transform.position}");
        player_.transform.position = playerSpawnDungeonPart.spawnPoint.position;
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

    //TODO player Setting


    // Update is called once per frame
    void Update()
    {
        
    }
}
