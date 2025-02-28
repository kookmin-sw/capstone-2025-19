using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_thorn : MonoBehaviour
{

    [SerializeField] private float raiseHeight = 0.5f;
    [SerializeField] private float raiseSpeed = 5.0f;
    [SerializeField] private float fallSpeed = 2.0f;
    [SerializeField] private float stayTime = 1.0f;
    private Transform groundThorn;

    public int damage = 15;

    private Vector3 originalPosition; 
    private bool isMoving = false;    

    private void Start()
    {
        groundThorn = transform.Find("Ground_16_2");
        originalPosition = groundThorn.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"인 오브젝트가 트리거에 진입
        if (other.CompareTag("Player") && !isMoving)
        {
            Debug.Log("player가 Ground thorn 함정 발동");
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            StartCoroutine(SpikeRoutine());
        }
    }

    private IEnumerator SpikeRoutine()
    {
        isMoving = true;

        // 올라갈 목표 위치(현재 위치 + 위쪽으로 raiseHeight만큼)
        Vector3 targetPosition = originalPosition + Vector3.up * raiseHeight;

        // 1) 위로 빠르게 상승
        while (Vector3.Distance(groundThorn.position, targetPosition) > 0.01f)
        {
            groundThorn.position = Vector3.MoveTowards(
                groundThorn.position,
                targetPosition,
                raiseSpeed * Time.deltaTime
            );
            yield return null; // 한 프레임 쉬고 다시 반복
        }

        // 2) 잠깐 유지
        yield return new WaitForSeconds(stayTime);

        // 3) 아래로 천천히 하강
        while (Vector3.Distance(groundThorn.position, originalPosition) > 0.01f)
        {
            groundThorn.position = Vector3.MoveTowards(
                groundThorn.position,
                originalPosition,
                fallSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 움직임 완료
        isMoving = false;
    }
}
