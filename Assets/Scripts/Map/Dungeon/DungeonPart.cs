using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
    //�� �ϳ��ϳ��� ���� ������Ʈ ��ũ��Ʈ
    public enum DungeonPartType
    {
        Room,
        Hallway,
        Stair,
        SpecialRoom,

    }

    public enum RoomUse
    {
        Nothing,
        PlayerSpawn,
        MonsterSpawn,

    }
    [SerializeField] private LayerMask roomsLayermask;
    [SerializeField] public DungeonPartType dungeonPartType;
    [SerializeField] GameObject fillerWall; 
    [SerializeField] public RoomUse roomUse;
    [SerializeField] List<ItemRandomSpawner> spawnItemList;
    [SerializeField] List<MonsterRandomSpawner> spawnMonsterList;
    [SerializeField] public List<Collider> colliderList;
    [SerializeField] public List<Transform> playerSpawnPoints;
    
    // chest object randomSpawn List
    //TODO EnemySpawn Point functiion
    public List<Transform> entryPoints;
    //public new Collider collider; 
                     
    public bool HasAvailableEntryPoint(out Transform entrypoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 100; 
        int retryIndex = 0;

        if(entryPoints.Count == 1) 
        {
            Transform entry = entryPoints[0]; 
            if(entry.TryGetComponent<EntryPoint>(out EntryPoint res)) 
            {
                if (res.IsOccupied())
                {
                    result = false;
                    resultingEntry = null; 
                }
                else
                {
                    result = true;
                    resultingEntry = entry; 
                    res.SetOccupied();  
                }
                entrypoint = resultingEntry;
                return result;
            }
        }
        while (resultingEntry == null && retryIndex < totalRetries) { 
            
            int randomEntryIndex = UnityEngine.Random.Range(0, entryPoints.Count);
            Transform entry = entryPoints[randomEntryIndex];
            if(entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())//�Ҵ� �ȵ� ���
                {
                    //�Ҵ��Ű��
                    resultingEntry = entry;
                    result = true;
                    entryPoint.SetOccupied();
                    break;
                }
            }
            retryIndex++;
        }
        entrypoint = resultingEntry;
        return result;
    }
    
    public void UnuseEntrypoint(Transform entrypoint)//������ entrypoint�� �Ҵ��� �����ߴٰ� ���� ������ ���̳� ������ ���� ���ϰ� �� ��� �ٽ� �Ⱦ��ٰ� �����ϴ� �޼��� �ε�
    {
        if(entrypoint.TryGetComponent<EntryPoint>(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }
    
    public void FillEmptyDoors()//�Ⱦ��� entry�� ������ ����� �޼���
    {
        entryPoints.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(fillerWall);
                    //wall.GetComponent<NetworkObject>().Spawn(true);//��Ƽ�÷��̽ÿ� �ʿ��� �ڵ�. PhotonNetwork.Instatiate()�� �ؾ� �ҵ�
                    //entry ����Ʈ�� �־���� �ű� ������ ��ǥ�� Ʋ���� �� ���� (�������� ũ�⸦ ���� �ø��� �������� entry�� �����ұ�?)
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }

    void OnDrawGizmos()
    {
        foreach(Collider collider in colliderList)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
        
    }

    /*public NetWorkObject GetNetworkObject() //��Ƽ�÷��� ���
    {
        return NetworkObject;
    }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public bool IsColliderOverlapping()
    {
        if (GetComponent<Collider>() == null) return false;

        Collider[] hits = Physics.OverlapBox(
            GetComponent<Collider>().bounds.center,
            GetComponent<Collider>().bounds.extents,
            Quaternion.identity,
            roomsLayermask
        );

        foreach (Collider hit in hits)
        {
            if (hit != GetComponent<Collider>() && hit.gameObject.layer == GetComponent<Collider>().gameObject.layer)
            {
            }
        }

        return false;
    }
    /*void OnDrawGizmos()
    {
        if (GetComponent<Collider>() != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.size);
        }
    }*/

    public List<Transform> GetEntryPointList()
    {
        List<Transform> emptyList = new List<Transform>();
        foreach(Transform entry in entryPoints)
        {
            if(!entry.GetComponent<EntryPoint>().IsOccupied()) emptyList.Add(entry);
        }
        return emptyList;
    }

    public void SpawnItem()
    {
        foreach(ItemRandomSpawner spawner in spawnItemList)
        {
            spawner.SpawnItem();
        }
    }

    public void SpawnMonster()
    {
        //TODO Monster Spawner
        //if (roomUse == RoomUse.PlayerSpawn) { return; }
        foreach(MonsterRandomSpawner spawner in spawnMonsterList)
        {
            spawner.SetSpawn();
        }
    }

    public void SpawnObject()
    {
        //TODO Obeject Spawner
    }
    
}
