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
    }
    [SerializeField] private LayerMask roomsLayermask;
    [SerializeField] private DungeonPartType dungeonPartType;
    [SerializeField] GameObject fillerWall; //������ �� �����ǰ� ������ ���� �� �Ա��� �̰ɷ� ä�� -> ������ ����� ������ �Ҵ��ؾ� ��

    public List<Transform> entryPoints; // �濡 �ִ� ��
    public new Collider collider; //new�� ��� :�θ��� ���� �̸��� ���� ������Ʈ�� ����. �θ� Ŭ������ Collider collider�� ������ �װŸ� base.collider�� ���� �ϰ� ���ο� collider�� ����.
                                  //�� collider�� ���� ������ ���� ���� ������ ��� ��ġ�� �����Ǵ��� Ȯ���ϴ� �뵵. �׳� �ܼ��� box collider���� ��(Trigger)
                                  //���� �Ա��� ������ ����� �ȵ�(colldier�� �� ũ�⺸�� �̹��ϰ� �۾ƾ� ��) -> �׷��� �Ա��� ��ƴ�� ����ϱ�? 
                                  // entry�� ��ġ�� collider�� �ۿ� �־�� ��
                     
    public bool HasAvailableEntryPoint(out Transform entrypoint)//Entrypoint���� Ȯ���ϰ� �Ҵ� �ȵ�(�ٸ� ��, ������ ������ �ȵ�) entryPoint�� �����ϴ� 
    {
        Transform resultingEntry = null;
        bool result = false;//ã�Ҵ��� �ƴ��� ���

        int totalRetries = 100; //������ġ
        int retryIndex = 0;

        if(entryPoints.Count == 1) //�濡 �Ա��� �ϳ��� ���. -> �� ���� ��. �׳� �Ա� 2�� �̻� ����� ���� �������� �Ա� ����� �������� ���� ���� (�������� ���� ��쿡�� �Ա��� �ϳ�?)
        {
            Transform entry = entryPoints[0]; // �Ա� �ϳ��ϱ� �׳� ���ϰ� ����
            if(entry.TryGetComponent<EntryPoint>(out EntryPoint res)) // �Ա� ������Ʈ�� EntryPoint �ް� (������Ʈ �ִ����� ���ÿ� Ȯ��)
            {
                if (res.IsOccupied())//�Ҵ� �Ǿ����� Ȯ��
                {
                    //�̹� �Ҵ� �� entry�� ����� false
                    result = false;
                    resultingEntry = null; //���� entry�� ������ null �Ҵ�
                }
                else
                {
                    //�Ҵ� �ȵ� entry�� ��� true
                    result = true;
                    resultingEntry = entry; // �Ҵ� �ȵ� entry �Ҵ�
                    res.SetOccupied();  //�� entry �Ҵ� �Ǿ��ٰ� ����
                }
                entrypoint = resultingEntry;
                return result;
            }
        }
        //�濡 �Ա��� �ϳ��� �ƴ� ��� � �Ա��� ���� ���� ��
        while (resultingEntry == null && retryIndex < totalRetries)// ���������� �� entry ���������� ���� (���ѷ��� ��������� 100�� �ݺ�����)
        {
            //�ٵ� �̷��� �Ҵ�� entry�� Ȯ���ϴٰ� ���� �� ���� �ʳ�? <- �̰� �� ���� ���� ������ ���÷�����
            int randomEntryIndex = UnityEngine.Random.Range(0, entryPoints.Count);//�������� �Ա� �ϳ� ����
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

    /*void OnDrawGizmos() //Unity Scene �信�� ������� ���� ����� �׸��� �Լ�. ������ ������� �ʾƵ� Scene �信�� �ð������� ������ ǥ���ϴµ� �����
    {
        //�� boxCollider(������ �� ũ��)�� ����� ������ ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }*/

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
        /*if (IsColliderOverlapping())
        {
            Debug.Log("���� Layer�� Collider�� ���ο� �ֽ��ϴ�!");
        }
        else
        {
            Debug.Log("��ġ�� collider ����");
        }*/
    }
    public bool IsColliderOverlapping()
    {
        if (collider == null) return false;

        // OverlapBox�� �浹 ���� (bounds.extents ���)
        Collider[] hits = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            Quaternion.identity,
            roomsLayermask
        );

        foreach (Collider hit in hits)
        {
            // �ڱ� �ڽ��� �����ϰ� ���� Layer���� Ȯ��
            if (hit != collider && hit.gameObject.layer == collider.gameObject.layer)
            {
                return true; // ���� Layer�� Collider�� ����
            }
        }

        return false;
    }
    void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }

    public List<Transform> GetEntryPointList()
    {
        List<Transform> emptyList = new List<Transform>();
        foreach(Transform entry in entryPoints)
        {
            if(!entry.GetComponent<EntryPoint>().IsOccupied()) emptyList.Add(entry);
        }
        return emptyList;
    }
    
}
