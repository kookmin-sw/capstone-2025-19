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
        rb.isKinematic = true;    // 직접 위치 제어
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void Shot()
    {
        // 발사 타이밍에 코루틴 시작
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        float elapsed = 0f;
        Vector3 dir = transform.forward;   // 발사 순간 방향 고정

        while (elapsed < lifeTime)
        {
            Vector3 next = transform.position + dir * speed * Time.fixedDeltaTime;
            rb.MovePosition(next);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate(); // 물리 프레임마다 한 번
        }

        Destroy(gameObject);  // 풀링이면 SetActive(false)
    }
}
