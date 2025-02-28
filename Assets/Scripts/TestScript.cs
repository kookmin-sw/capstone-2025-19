using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider targetCollider; // 검사할 Collider
    public LayerMask layerMask; // 감지할 Layer 설정
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsColliderOverlapping(targetCollider, layerMask))
        {
            Debug.Log("같은 Layer의 Collider가 내부에 있습니다!");
        }
        else
        {
            Debug.Log("곂치는 collider 없음");
        }
    }
    bool IsColliderOverlapping(Collider target, LayerMask layerMask)
    {
        if (target == null) return false;

        // OverlapBox로 충돌 감지 (bounds.extents 사용)
        Collider[] hits = Physics.OverlapBox(
            target.bounds.center,
            target.bounds.extents,
            Quaternion.identity,
            layerMask
        );

        foreach (Collider hit in hits)
        {
            // 자기 자신은 제외하고 같은 Layer인지 확인
            if (hit != target && hit.gameObject.layer == target.gameObject.layer)
            {
                return true; // 같은 Layer의 Collider가 있음
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (targetCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(targetCollider.bounds.center, targetCollider.bounds.size);
        }
    }
}
