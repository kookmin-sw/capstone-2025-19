using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider targetCollider; // �˻��� Collider
    public LayerMask layerMask; // ������ Layer ����
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsColliderOverlapping(targetCollider, layerMask))
        {
            Debug.Log("���� Layer�� Collider�� ���ο� �ֽ��ϴ�!");
        }
        else
        {
            Debug.Log("��ġ�� collider ����");
        }
    }
    bool IsColliderOverlapping(Collider target, LayerMask layerMask)
    {
        if (target == null) return false;

        // OverlapBox�� �浹 ���� (bounds.extents ���)
        Collider[] hits = Physics.OverlapBox(
            target.bounds.center,
            target.bounds.extents,
            Quaternion.identity,
            layerMask
        );

        foreach (Collider hit in hits)
        {
            // �ڱ� �ڽ��� �����ϰ� ���� Layer���� Ȯ��
            if (hit != target && hit.gameObject.layer == target.gameObject.layer)
            {
                return true; // ���� Layer�� Collider�� ����
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
