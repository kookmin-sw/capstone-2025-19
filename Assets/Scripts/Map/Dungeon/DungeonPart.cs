using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
    //방 하나하나에 들어가는 컴포넌트 스크립트
    public enum DungeonPartType
    {
        Room,
        Hallway,
        Stair,
    }
    [SerializeField] private LayerMask roomsLayermask;
    [SerializeField] private DungeonPartType dungeonPartType;
    [SerializeField] GameObject fillerWall; //던전이 다 생성되고 사용되지 않은 빈 입구를 이걸로 채움 -> 재질이 비슷한 벽으로 할당해야 함

    public List<Transform> entryPoints; // 방에 있는 문
    public new Collider collider; //new의 기능 :부모의 같은 이름을 가진 오브젝트를 숨김. 부모 클래스에 Collider collider가 있으면 그거를 base.collider로 쓰게 하고 새로운 collider를 생성.
                                  //이 collider는 지금 생성된 방이 이전 생성된 방과 곂치게 생성되는지 확인하는 용도. 그냥 단순한 box collider여도 됨(Trigger)
                                  //결코 입구를 완전히 덮어서는 안됨(colldier는 방 크기보다 미묘하게 작아야 함) -> 그러면 입구에 빈틈이 생기니까? 
                                  // entry의 위치가 collider의 밖에 있어야 함
                     
    public bool HasAvailableEntryPoint(out Transform entrypoint)//Entrypoint들을 확인하고 할당 안된(다른 방, 복도와 연결이 안된) entryPoint를 리턴하는 
    {
        Transform resultingEntry = null;
        bool result = false;//찾았는지 아닌지 결과

        int totalRetries = 100; //안전장치
        int retryIndex = 0;

        if(entryPoints.Count == 1) //방에 입구가 하나인 경우. -> 난 없을 듯. 그냥 입구 2개 이상 만들어 놓고 마지막에 입구 지우는 형식으로 갈것 같음 (폴가이즈 방의 경우에는 입구가 하나?)
        {
            Transform entry = entryPoints[0]; // 입구 하나니까 그냥 정하고 시작
            if(entry.TryGetComponent<EntryPoint>(out EntryPoint res)) // 입구 컴포넌트인 EntryPoint 받고 (컴포넌트 있는지도 동시에 확인)
            {
                if (res.IsOccupied())//할당 되었는지 확인
                {
                    //이미 할당 된 entry면 결과는 false
                    result = false;
                    resultingEntry = null; //보낼 entry도 없으니 null 할당
                }
                else
                {
                    //할당 안된 entry면 결과 true
                    result = true;
                    resultingEntry = entry; // 할당 안된 entry 할당
                    res.SetOccupied();  //이 entry 할당 되었다고 변경
                }
                entrypoint = resultingEntry;
                return result;
            }
        }
        //방에 입구가 하나가 아닐 경우 어떤 입구를 쓸지 골라야 함
        while (resultingEntry == null && retryIndex < totalRetries)// 최종적으로 쓸 entry 정해지기전 까지 (무한루프 막기용으로 100번 반복까지)
        {
            //근데 이러면 할당된 entry만 확인하다가 끝날 수 있지 않나? <- 이거 더 좋은 생각 있으면 떠올려보기
            int randomEntryIndex = UnityEngine.Random.Range(0, entryPoints.Count);//랜덤으로 입구 하나 고르기
            Transform entry = entryPoints[randomEntryIndex];
            if(entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())//할당 안된 경우
                {
                    //할당시키기
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
    
    public void UnuseEntrypoint(Transform entrypoint)//쓰려고 entrypoint에 할당함 설정했다가 여러 이유로 방이나 복도를 설정 못하게 된 경우 다시 안쓴다고 설정하는 메서드 인듯
    {
        if(entrypoint.TryGetComponent<EntryPoint>(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }
    
    public void FillEmptyDoors()//안쓰는 entry를 벽으로 만드는 메서드
    {
        entryPoints.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(fillerWall);
                    //wall.GetComponent<NetworkObject>().Spawn(true);//멀티플레이시에 필요한 코드. PhotonNetwork.Instatiate()로 해야 할듯
                    //entry 포인트에 넣어놓은 거기 때문에 좌표가 틀어질 수 있음 (문지방의 크기를 조금 늘리고 문지방을 entry로 설정할까?)
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }

    /*void OnDrawGizmos() //Unity Scene 뷰에서 디버깅을 위해 기즈모를 그리는 함수. 게임이 실행되지 않아도 Scene 뷰에서 시각적으로 정보를 표시하는데 사용함
    {
        //이 boxCollider(실질적 방 크기)를 노란색 선으로 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }*/

    /*public NetWorkObject GetNetworkObject() //멀티플레이 요소
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
            Debug.Log("같은 Layer의 Collider가 내부에 있습니다!");
        }
        else
        {
            Debug.Log("곂치는 collider 없음");
        }*/
    }
    public bool IsColliderOverlapping()
    {
        if (collider == null) return false;

        // OverlapBox로 충돌 감지 (bounds.extents 사용)
        Collider[] hits = Physics.OverlapBox(
            collider.bounds.center,
            collider.bounds.extents,
            Quaternion.identity,
            roomsLayermask
        );

        foreach (Collider hit in hits)
        {
            // 자기 자신은 제외하고 같은 Layer인지 확인
            if (hit != collider && hit.gameObject.layer == collider.gameObject.layer)
            {
                return true; // 같은 Layer의 Collider가 있음
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
