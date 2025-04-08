using System.Collections;
using UnityEngine;

/// <summary>
/// 일정 간격마다 지정된 방향으로 '튀어나왔다가' 다시 원점으로 돌아가는 함정 스크립트.
/// </summary>
public class PushTrap : MonoBehaviour
{
    // 함정이 밀어올리는(또는 미는) 거리
    [SerializeField] private float pushDistance = 0.5f;

    // 올리는(발사) 속도와 다시 돌아오는(하강) 속도
    [SerializeField] private float pushSpeed = 5.0f;
    [SerializeField] private float returnSpeed = 2.0f;

    // 발사 후 잠시 유지하는 시간
    [SerializeField] private float stayTime = 1.0f;

    // 발사를 반복할 간격
    [SerializeField] private float intervalTime = 3.0f;

    // 데미지 (상황에 따라 사용)
    [SerializeField] private int damage = 15;

    // 발사 방향을 enum으로 구성
    public enum PushDirection
    {
        Up,
        Down,
        Left,
        Right
        // 필요하면 Forward, Backward 등을 추가 가능
    }

    // Inspector에서 방향 선택
    [SerializeField] private PushDirection direction = PushDirection.Up;

    // 내부용
    private Vector3 originalPosition;
    private bool isMoving = false;

    private void Start()
    {
        // 현재 위치 기억
        originalPosition = transform.position;

        // 일정 간격마다 PushRoutine을 실행하는 코루틴
        StartCoroutine(AutoPushRoutine());
    }

    /// <summary>
    /// 주기적으로 함정을 발사시키는 코루틴
    /// </summary>
    private IEnumerator AutoPushRoutine()
    {
        while (true)
        {
            // IntervalTime만큼 대기 후
            yield return new WaitForSeconds(intervalTime);

            // 이미 움직이고 있지 않다면 발사
            if (!isMoving)
            {
                StartCoroutine(PushRoutine());
            }
        }
    }

    /// <summary>
    /// 함정이 발사(올라감) -> 유지 -> 다시 복귀하는 동작
    /// </summary>
    private IEnumerator PushRoutine()
    {
        isMoving = true;

        // 발사 방향 벡터 구하기
        Vector3 pushDir = GetPushDirection(direction);

        // 올라갈 목표 위치(= 원래 위치 + 발사방향 * 거리)
        Vector3 targetPosition = originalPosition + pushDir * pushDistance;

        // 1) 밀어올림(또는 미는 동작)
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                pushSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 2) 잠시 유지
        yield return new WaitForSeconds(stayTime);

        // 3) 원래 위치로 되돌아감
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPosition,
                returnSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 움직임 완료
        isMoving = false;
    }

    /// <summary>
    /// enum에 따라 실제 월드 방향 벡터를 반환
    /// </summary>
    private Vector3 GetPushDirection(PushDirection dir)
    {
        switch (dir)
        {
            case PushDirection.Up:
                return Vector3.up;
            case PushDirection.Down:
                return Vector3.down;
            case PushDirection.Left:
                return Vector3.left;
            case PushDirection.Right:
                return Vector3.right;
            // 필요한 경우 Forward, Back, 등 더 추가 가능
            default:
                return Vector3.up; // 기본값
        }
    }
}
