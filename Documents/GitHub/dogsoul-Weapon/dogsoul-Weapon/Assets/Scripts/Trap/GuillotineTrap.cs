using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuillotineTrap : MonoBehaviour
{
    [SerializeField] GameObject knife;
    [SerializeField] private float fallSpeed = 5.0f;
    [SerializeField] private float upSpeed = 2.0f;
    [SerializeField] private float fallHeight = 2.0f;
    [SerializeField] private float stayTime = 1.0f;
    [SerializeField] private float IntervalTime = 15.0f;

    private Vector3 originalPosition;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = knife.transform.position;
        StartCoroutine(KnifeDropByInterval());
    }

    // 일정 간격마다 KnifeDropRoutine을 호출해주는 코루틴
    private IEnumerator KnifeDropByInterval()
    {
        while (true)
        {
            // IntervalTime만큼 대기 후
            yield return new WaitForSeconds(IntervalTime);

            // 이미 움직이고 있지 않다면 KnifeDropRoutine 실행
            if (!isMoving)
            {
                StartCoroutine(KnifeDropRoutine());
            }
        }
    }

    private IEnumerator KnifeDropRoutine()
    {
        isMoving = true;

        // 내려갈 목표 위치(현재 위치 + 위쪽으로 raiseHeight만큼)
        Vector3 targetPosition = originalPosition + Vector3.down * fallHeight;

        // 1) 아래로 빠르게 하강
        while (Vector3.Distance(knife.transform.position, targetPosition) > 0.01f)
        {
            knife.transform.position = Vector3.MoveTowards(
                knife.transform.position,
                targetPosition,
                fallSpeed * Time.deltaTime
            );
            yield return null; // 한 프레임 쉬고 다시 반복
        }

        // 2) 잠깐 유지
        yield return new WaitForSeconds(stayTime);

        // 3) 위로 천천히 상승
        while (Vector3.Distance(knife.transform.position, originalPosition) > 0.01f)
        {
            knife.transform.position = Vector3.MoveTowards(
                knife.transform.position,
                originalPosition,
                upSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 움직임 완료
        isMoving = false;
    }
}
