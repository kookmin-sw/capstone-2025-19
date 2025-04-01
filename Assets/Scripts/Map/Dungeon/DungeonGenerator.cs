using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


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
    //���� ����
    public void StartGeneration()
    {
        StartGenerationServerRpc();
    }

    //[ServerRpc(RequireOwnership = false)]
    private void StartGenerationServerRpc()
    {
        Generate(); //���� ����
        //GenerateAlternateEntrances(); //�ٸ� �Ա� ���� -> �ϴ� ����. �˰��� ��ü�� ���� �� ������ �Ȱ���
        //FillEmptyEntrances(); //��� ���� ������ ���� ���� �Ա� ������ ����

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
                

                /*if (shouldPlaceHallway)//50% Ȯ���� ������ ��ġ�ؾ� �� ��� ������ �� ���ַ� ��ġ �Ұ��� 
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject room2 = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//���� ����
                    room2.transform.SetParent(null);
                    if (room2.TryGetComponent<DungeonPart>(out DungeonPart room2_dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // ��Ƽ�÷��� ���
                        if (room2_dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//������ ������ ������ �Ա� ����
                        {
                            while (room1 == null && retryIndex < totalRetries) // ������ �ൿ while���� ���� Ȥ�� �𸣴� ���� ������ ���� ���ɼ��� ����� ���� �ִ� 100�������� ���� ���� ��
                            {
                                int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //������ ���� ����
                                DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                                if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//�� ������ �濡�� �Ա��� ã�� ������ room1EntryPoint�� �Ҵ��ϰ� 
                                {
                                    room1 = roomToTest;//randomGeneratedRoom�� ������ ���� �Ҵ� �� while�� break
                                    break;
                                }
                                retryIndex++; //������ġ
                            }
                            // ������ �濡�� ������ �� ����
                            *//*foreach(DungeonPart dungeonPart in generatedRooms)
                            {

                            }
                            if(room1.HasAvailableEntryPoint(out room1EntryPoint))*//*


                            AlignRooms(room1.transform, room2.transform, room1EntryPoint, room2Entrypoint); //�� ���� (ù��° ��, ���� ������ ����,
                                                                                                            //ù��° �� �Ա�, ���� ������ ���� �Ա�)

                            //if (HandleIntersection(room2_dungeonPart)) //��ġ�� �̸� ������ ���̶� ��ģ�ٴ��� �ϴ� ������ ���� �� �ǵ����� �κ��ΰ� ������
                            bool innerTest = false;
                            if (room2_dungeonPart.IsColliderOverlapping())
                            {
                                room2_dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                room1.UnuseEntrypoint(room1EntryPoint);
                                //�׷��� ������ �� ���� ����?
                                //�ϴ� ������
                                Debug.Log("���� ��ħ");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //�ϴ� ����Լ��� ���ο� ��, ���ο� �Ա� ã��
                                //TODO �׳ɸ�� ��, �Ա��� �ϳ��� ������ ���鼭 �ֱ�
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
                                                generatedRooms.Add(room2_dungeonPart);//������ �������� �˸��� �Ա��� ������� ������ ������ generatedRooms(������ �� ����Ʈ)�� �߰�
                                                Debug.LogError("ȭ�� ����");
                                                room1Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                room2Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                check1 = true;
                                                check2 = true;
                                                finalCheck = true;
                                                innerTest = true;
                                                Debug.Log("���");
                                                break;
                                            }
                                            Debug.Log("room2(Hallway)�� �Ա� �ٲٱ�");
                                        }
                                        if (check2) break;
                                        Debug.Log("room1�� �Ա� �ٲٱ�");
                                    }
                                    if (check1) break;
                                    Debug.Log("room1�� �ٲٱ�");
                                }
                                if (!finalCheck)
                                {
                                    Debug.Log("room2(Hallway)�� �ٲٱ� !!! ");
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
                    GameObject room2 = Instantiate(rooms[randomIndex], transform.position, transform.rotation);//���� ����
                    room2.transform.SetParent(null);
                    if (room2.TryGetComponent<DungeonPart>(out DungeonPart room2_dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // ��Ƽ�÷��� ���
                        if (room2_dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//������ ������ ������ �Ա� ����
                        {
                            while (room1 == null && retryIndex < totalRetries) // ������ �ൿ while���� ���� Ȥ�� �𸣴� ���� ������ ���� ���ɼ��� ����� ���� �ִ� 100�������� ���� ���� ��
                            {
                                int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //������ ���� ����
                                DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                                if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//�� ������ �濡�� �Ա��� ã�� ������ room1EntryPoint�� �Ҵ��ϰ� 
                                {
                                    room1 = roomToTest;//randomGeneratedRoom�� ������ ���� �Ҵ� �� while�� break
                                    break;
                                }
                                retryIndex++; //������ġ
                            }
                            // ������ �濡�� ������ �� ����
                            *//*foreach(DungeonPart dungeonPart in generatedRooms)
                            {

                            }
                            if(room1.HasAvailableEntryPoint(out room1EntryPoint))*//*


                            AlignRooms(room1.transform, room2.transform, room1EntryPoint, room2Entrypoint); //�� ���� (ù��° ��, ���� ������ ����,
                                                                                                            //ù��° �� �Ա�, ���� ������ ���� �Ա�)
                            Debug.Log("test");
                            bool innerTest = false;
                            if (room2_dungeonPart.IsColliderOverlapping())
                            //if (HandleIntersection(room2_dungeonPart)) //��ġ�� �̸� ������ ���̶� ��ģ�ٴ��� �ϴ� ������ ���� �� �ǵ����� �κ��ΰ� ������
                            {
                                room2_dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                room1.UnuseEntrypoint(room1EntryPoint);
                                //�׷��� ������ �� ���� ����?
                                //�ϴ� ������
                                Debug.Log("���� ��ħ");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //�ϴ� ����Լ��� ���ο� ��, ���ο� �Ա� ã��
                                //TODO �׳ɸ�� ��, �Ա��� �ϳ��� ������ ���鼭 �ֱ�
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
                                                generatedRooms.Add(room2_dungeonPart);//������ �������� �˸��� �Ա��� ������� ������ ������ generatedRooms(������ �� ����Ʈ)�� �߰�
                                                Debug.LogError("ȭ�� ����");
                                                room1Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                //room2Entrypoint
                                                room2Transform.GetComponent<EntryPoint>().SetOccupied(true);
                                                check1 = true;
                                                check2 = true;
                                                finalCheck = true;
                                                innerTest = true;
                                                Debug.Log("���");
                                                break;
                                            }
                                            Debug.Log("room2(Hallway)�� �Ա� �ٲٱ�");
                                        }
                                        if (check2) break;
                                        Debug.Log("room1�� �Ա� �ٲٱ�");
                                    }
                                    if (check1) break;
                                    Debug.Log("room1�� �ٲٱ�");
                                }
                                if (!finalCheck)
                                {
                                    Debug.LogError("room2(room)�� �ٲٱ� !!! ");
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
        Debug.Log("�Ϸ�");
    }
    /*private void TestGenerate() //�ϴ� ���� ���ϴ� ���� ������ �Ѿ ���� ����� ���� ���� -> �׷� �̷��� �Ǹ� �̸� ����� ���� ���� �� ���� �� �̻��� ����?
    {
        for(int i = 0; i < noOfRooms - alternateEntrances.Count; i++)
        {
            if(generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //���⿡ �ִ� entrance�� ������ �������� �ɰ���
                generatedRoom.transform.SetParent(null);// ������ ���� � parent�� ���� �ִٸ� Position ���� parent���� ���� ���� �� �ֱ⿡ Ȥ�ó� parent �����Ǿ������� null�� �Ҵ��ϱ�
                
                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    //��Ƽ�÷��� �����
                    //dungeonPart.GetNetworkObject().Spawn(true); //��� Ŭ���̾�Ʈ�� ���� ���� ��ü�� �����ϴ� -> photonView.Instantiate()����
                    generatedRooms.Add(dungeonPart); //�� ����� List�� �߰�
                }
            }
            else
            {
                bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.5f; //������ ������ Ȯ�� 50%
                DungeonPart randomGeneratedRoom = null; //�̹� ���� ���ο� �������� ������ ��
                Transform room1EntryPoint = null; // ������ ������ ���� �������� �� ������.
                                                  // �⺻������ ��A�� ��B�� ���� �� �� A�� ��B�� ���� �� �� �ֱ⿡ �� ��A(���� ��)�� ������(�Ա�, ��)�� ��Ÿ���� ����Ʈ

                int totalRetries = 100; //��ü ��Ʈ���� Ƚ��. ������ġ��µ�
                int retryIndex = 0;


                while (randomGeneratedRoom == null && retryIndex < totalRetries) // ������ �ൿ while���� ���� Ȥ�� �𸣴� ���� ������ ���� ���ɼ��� ����� ���� �ִ� 100�������� ���� ���� ��
                {
                    int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //������ ������ ���� ����
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                    if(roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//�� ������ �濡�� �Ա��� ã�� ������ room1EntryPoint�� �Ҵ��ϰ� 
                    {
                        randomGeneratedRoom = roomToTest;//randomGeneratedRoom�� ������ ���� �Ҵ� �� while�� break
                        break;
                    }
                    retryIndex++; //������ġ
                }
                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
                //doorToAlign.GetComponent<NetworkObject>().Spawn(true); // ��Ƽ�÷��̿��

                if (shouldPlaceHallway)//50%Ȯ���� ������ ��ġ�ؾ� �� ���
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//���� ����
                    generatedHallway.transform.SetParent(null);
                    if(generatedHallway.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // ��Ƽ�÷��� ���
                        if(dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//������ ������ ������ �Ա� ����
                        {
                            generatedRooms.Add(dungeonPart);//������ �������� �˸��� �Ա��� ������� ������ ������ generatedRooms(������ �� ����Ʈ)�� �߰�
                            doorToAlign.transform.position = room1EntryPoint.transform.position;//�´� ��ġ�� ���߱�
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2Entrypoint); //�� ���� (ù��° ��, ���� ������ ����,
                                                                                                                                     //ù��° �� �Ա�, ���� ������ ���� �Ա�)
                            if (HandleIntersection(dungeonPart)) //��ġ�� �̸� ������ ���̶� ��ģ�ٴ��� �ϴ� ������ ���� �� �ǵ����� �κ��ΰ� ������
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                //�׷��� ������ �� ���� ����?
                                //�ϴ� ������
                                Debug.Log("���� ��ħ");
                                //Destroy(dungeonPart.gameObject);
                                //RetryPlacement(generatedHallway, doorToAlign); //�ϴ� ����Լ��� ���ο� ��, ���ο� �Ա� ã��
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;
                    if(specialRooms.Count > 0) // Ư���� ���� ���� ������ ���� ���
                    {
                        bool shouldPlaceSpecialRoom = UnityEngine.Random.Range(0f, 1f) > 0.9f;//10���� Ȯ���� Ư���� ���� ������ -> �� �׳� �ϳ��� ����� �ҵ�
                        if(shouldPlaceSpecialRoom)
                        {
                            int randomIndex = UnityEngine.Random.Range(0, specialRooms.Count); //Ư���� �� �����
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = UnityEngine.Random.Range(0, rooms.Count); //����� �� �����
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else // �ƴ� ��� (Ư���� �� ���� ���� ����)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                    }

                    generatedRoom.transform.SetParent(null);
                    if(generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))//������ �� ��ġ ����
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); //��Ƽ�÷��� ���
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
                                //�ϴ� �� ��ġ�� �� ������
                                Debug.Log("�� ��ħ");
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
            if (generatedRooms.Count < 1) //���� ������ ���� ���ٸ�
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation); //���⿡ �ִ� entrance�� ������ �������� �ɰ���
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
                bool shouldPlaceHallway = UnityEngine.Random.Range(0f, 1f) > 0.5f; //������ ������ Ȯ�� 50%
                DungeonPart randomGeneratedRoom = null; //�̹� ���� ���ο� �������� ������ ��
                Transform room1EntryPoint = null; // ������ ������ ���� �������� �� ������.
                                                  // �⺻������ ��A�� ��B�� ���� �� �� A�� ��B�� ���� �� �� �ֱ⿡ �� ��A(���� ��)�� ������(�Ա�, ��)�� ��Ÿ���� ����Ʈ

                int totalRetries = 100; //��ü ��Ʈ���� Ƚ��. ������ġ��µ�
                int retryIndex = 0;


                while (randomGeneratedRoom == null && retryIndex < totalRetries) // ������ �ൿ while���� ���� Ȥ�� �𸣴� ���� ������ ���� ���ɼ��� ����� ���� �ִ� 100�������� ���� ���� ��
                {
                    int randomLinkRoomIndex = UnityEngine.Random.Range(0, generatedRooms.Count); //������ ���� ����
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                    if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))//�� ������ �濡�� �Ա��� ã�� ������ room1EntryPoint�� �Ҵ��ϰ� 
                    {
                        randomGeneratedRoom = roomToTest;//randomGeneratedRoom�� ������ ���� �Ҵ� �� while�� break
                        break;
                    }
                    retryIndex++; //������ġ
                }
                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
                //doorToAlign.GetComponent<NetworkObject>().Spawn(true); // ��Ƽ�÷��̿��

                if (shouldPlaceHallway)//50%Ȯ���� ������ ��ġ�ؾ� �� ���
                {
                    int randomIndex = UnityEngine.Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);//���� ����
                    generatedHallway.transform.SetParent(null);
                    if (generatedHallway.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); // ��Ƽ�÷��� ���
                        if (dungeonPart.HasAvailableEntryPoint(out Transform room2Entrypoint))//������ ������ ������ �Ա� ����
                        {
                            generatedRooms.Add(dungeonPart);//������ �������� �˸��� �Ա��� ������� ������ ������ generatedRooms(������ �� ����Ʈ)�� �߰�
                            doorToAlign.transform.position = room1EntryPoint.transform.position;//�´� ��ġ�� ���߱�
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2Entrypoint); //�� ���� (ù��° ��, ���� ������ ����,
                                                                                                                                     //ù��° �� �Ա�, ���� ������ ���� �Ա�)
                            if (HandleIntersection(dungeonPart)) //��ġ�� �̸� ������ ���̶� ��ģ�ٴ��� �ϴ� ������ ���� �� �ǵ����� �κ��ΰ� ������
                            {
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                //�׷��� ������ �� ���� ����?
                                RetryPlacement(generatedHallway, doorToAlign); //�ϴ� ����Լ��� ���ο� ��, ���ο� �Ա� ã��
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;
                    if (specialRooms.Count > 0) // Ư���� ���� ���� ������ ���� ���
                    {
                        bool shouldPlaceSpecialRoom = UnityEngine.Random.Range(0f, 1f) > 0.9f;//10���� Ȯ���� Ư���� ���� ������ -> �� �׳� �ϳ��� ����� �ҵ�
                        if (shouldPlaceSpecialRoom)
                        {
                            int randomIndex = UnityEngine.Random.Range(0, specialRooms.Count); //Ư���� �� �����
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = UnityEngine.Random.Range(0, rooms.Count); //����� �� �����
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else // �ƴ� ��� (Ư���� �� ���� ���� ����)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                    }

                    generatedRoom.transform.SetParent(null);
                    if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))//������ �� ��ġ ����
                    {
                        //dungeonPart.GetNetworkObject().Spawn(true); //��Ƽ�÷��� ���
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
    //���� �ڵ�
    /*private bool HandleIntersection(DungeonPart dungeonPart)// ������ ��(Ȥ�� ����)�� ���� ��ġ���� Ȯ��
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
            if (hit != dungeonPart.collider) // �ڱ� �ڽŰ� �浹�� �ƴ� ��츸 �˻�
            {
                didIntersect = true;
                break;
            }
        }
        return didIntersect;
    }*/

    private void AlignRooms(Transform room2, Transform room1Entry, Transform room2Entry) // room1�� room2�� �Ա��� ��Ȯ�ϰ� ��ġ�ϰ� ����°� room1�� ��� ���� room1Entry�� �ʿ�
    {
        //���� ���� �ڵ�
        /*float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);

        room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + angle, room2.eulerAngles.z);
        Vector3 offset = room1Entry.position - room2Entry.position;

        room2.position += offset;*/

        //�� �Ա��� ��ġ�� ��Ȯ�ϰ� ��ġ��Ŵ
        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;

        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.RotateAround(room2Entry.position, Vector3.up , angle);
        room2.RotateAround(room2Entry.position, Vector3.up, 180f);

        /*float angle = Vector3.SignedAngle(room2Entry.forward, -room1Entry.forward, Vector3.up);
        room2.Rotate(0, angle, 0, Space.World);*/


        Physics.SyncTransforms();//����Ƽ���� Collider�� ����ִ� ������Ʈ�� ������ �� ����� ����ȭ�� �ȵ� ���� ����
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
        //���� ������ PhotonView�� ���� ���� �ƴ�
        GameObject player = Instantiate(Resources.Load<GameObject>($"Prefabs/Player/DemoPlayer"));
        player.transform.localPosition = playerSpawnDungeonPart.spawnPoint.position;
        InventoryController.Instance.SetPlayer(player.transform.Find("Trigger").GetComponent<PlayerTrigger>());
        Debug.Log(player.transform.position);
        player_ = player;
        StartCoroutine(ResetPlayerPosition());
    }

    //���� ������ �÷��� �� �� ��� player�� position�� �������� ������ ã�� ������ ���
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
