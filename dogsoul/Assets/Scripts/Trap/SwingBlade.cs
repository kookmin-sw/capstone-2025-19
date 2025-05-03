using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBlade : MonoBehaviour
{
    [Header("Pendulum Settings")]
    public float length = 2.0f;            // �࿡�� Į�� �����߽ɱ����� �Ÿ� (����)
    public float gravity = 9.81f;         // �߷� ���ӵ�
    public float initialAngleDeg = 60f;   // ���� ���� (�� ����)

    [Header("Dynamics")]
    public float angleDeg;                // ���� ����(��). -�� ����, +�� ������ (����)
    private float angleVelocity = 0f;     // ���ӵ� (deg/s ������ �����ص� ��)

    

    void Start()
    {
        // ���� ���� ����
        angleDeg = initialAngleDeg;
    }
    

    void FixedUpdate()
    {
        // 1) �����ӵ� ��� (���� ���� ���)
        float angleRad = angleDeg * Mathf.Deg2Rad; // ���� ����(deg �� rad)

        // ��ⷳ � ������: ��'' = - (g / L) * sin(��)
        float angleAcceleration = -(gravity / length) * Mathf.Sin(angleRad);

        // 2) ���ӵ� ������Ʈ (deg/s�� �����Ϸ���, rad��deg ��ȯ)
        //    ���⼭�� "�ʴ� ���� ������"�� deg/s�� �ٲ㼭 ����
        angleVelocity += angleAcceleration * Time.fixedDeltaTime * Mathf.Rad2Deg;

        // 3) ���� ������Ʈ
        angleDeg += angleVelocity * Time.fixedDeltaTime;

        // 4) ������Ʈ(��)�� ȸ��
        //    ��: Z������ �¿� ����. (�� ������Ʈ ȸ���� �� �ڽ� Į���� ����)
        transform.localRotation = Quaternion.Euler(0f, 0f, angleDeg);
    }
}
