using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Trap : MonoBehaviour
{

    [SerializeField] private float rotateSpeedDegPerSec = 30f; // 초당 회전 각도
    [SerializeField] private float pushStrength = 1f;          // 표면 속도와의 오차를 얼마나 빠르게 보정할지

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Kinematic 설정
        //rb.isKinematic = true;
        //rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        // Kinematic 객체이므로 transform.Rotate()나 rb.MoveRotation()으로 직접 회전
        // 여기서는 X축 회전을 예시로, 초당 rotateSpeedDegPerSec만큼 회전
        transform.Rotate(Vector3.up, rotateSpeedDegPerSec * Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("In OnCollisionStay");
        // 충돌 상대가 Rigidbody를 갖고 있어야 물리적인 힘을 줄 수 있음
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb == null) {
            Debug.Log("player collision is null");
            return;
        }
        // 초당 회전 각도(deg/s)를 라디안(rad/s)으로 변환
        float angularSpeed = rotateSpeedDegPerSec * Mathf.Deg2Rad;
        // 충돌 지점마다 단순 계산
        foreach (ContactPoint contact in collision.contacts)
        {
            // 회전 중심(함정 위치)으로부터 접점까지의 거리 (반지름 r)
            float radius = Vector3.Distance(contact.point, transform.position);

            // 선속도( m/s ) ~= 각속도(rad/s) * 반지름(m)
            float surfaceLinearSpeed = angularSpeed * radius;

            // 원하는 힘(force) = (임의 계수) * surfaceLinearSpeed
            // 실제로는 질량·속도 차 등을 고려해야 하지만, 여기서는 간단화
            float forceValue = surfaceLinearSpeed * pushStrength;

            // 로컬 Z축 방향(앞쪽)으로 forceValue만큼 힘을 가함
            // (큐브의 로컬 forward 방향이 실제 “회전에 의해 밀리는 방향”이 맞는지 확인 필요)
            otherRb.AddRelativeForce(0f, 0f, (-1)*forceValue, ForceMode.Acceleration);
        }


    }
}
