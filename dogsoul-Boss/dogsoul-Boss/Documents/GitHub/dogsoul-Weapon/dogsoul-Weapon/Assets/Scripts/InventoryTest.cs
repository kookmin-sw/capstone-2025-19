using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform targetPosition;
    public float moveSpeed = 2f;

    void Start()
    {
        Invoke("StartMoving", 5f); // 5�� �� �̵� ����
    }

    void StartMoving()
    {
        StartCoroutine(MoveTowardsTarget());
    }

    IEnumerator MoveTowardsTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition.position; // ��Ȯ�� ��ġ ����
    }
}
