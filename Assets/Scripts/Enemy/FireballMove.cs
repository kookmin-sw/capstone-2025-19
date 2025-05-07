using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballMove : MonoBehaviour
{
    [SerializeField] float speed = 15f;
    [SerializeField] float lifeTime = 5f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;    // ���� ��ġ ����
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void Shot()
    {
        // �߻� Ÿ�ֿ̹� �ڷ�ƾ ����
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        float elapsed = 0f;
        Vector3 dir = transform.forward;   // �߻� ���� ���� ����

        while (elapsed < lifeTime)
        {
            Vector3 next = transform.position + dir * speed * Time.fixedDeltaTime;
            rb.MovePosition(next);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // ���� �����Ӹ��� �� ��
        }

        Destroy(gameObject);  // Ǯ���̸� SetActive(false)
    }
}
