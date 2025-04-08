using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBlade : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public float length = 2.0f;            // 축에서 칼날 무게중심까지의 거리 (미터)
    public float gravity = 9.81f;         // 중력 가속도
    public float initialAngleDeg = 60f;   // 시작 각도 (도 단위)

    [Header("Dynamics")]
    public float angleDeg;                // 현재 각도(도). -는 왼쪽, +는 오른쪽 (예시)
    private float angleVelocity = 0f;     // 각속도 (deg/s 단위로 관리해도 됨)

    

    void Start()
    {
        // 시작 각도 설정
        angleDeg = initialAngleDeg;
    }
    

    void FixedUpdate()
    {
        // 1) 각가속도 계산 (라디안 단위 사용)
        float angleRad = angleDeg * Mathf.Deg2Rad; // 현재 각도(deg → rad)

        // 펜듈럼 운동 방정식: θ'' = - (g / L) * sin(θ)
        float angleAcceleration = -(gravity / length) * Mathf.Sin(angleRad);

        // 2) 각속도 업데이트 (deg/s로 관리하려면, rad→deg 변환)
        //    여기서는 "초당 라디안 증가분"을 deg/s로 바꿔서 누적
        angleVelocity += angleAcceleration * Time.fixedDeltaTime * Mathf.Rad2Deg;

        // 3) 각도 업데이트
        angleDeg += angleVelocity * Time.fixedDeltaTime;

        // 4) 오브젝트(축)을 회전
        //    예: Z축으로 좌우 스윙. (축 오브젝트 회전이 곧 자식 칼날의 스윙)
        transform.localRotation = Quaternion.Euler(0f, 0f, angleDeg);
    }
}
