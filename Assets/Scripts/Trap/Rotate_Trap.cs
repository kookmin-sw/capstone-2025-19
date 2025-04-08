using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Trap : MonoBehaviour
{

    [SerializeField] private float rotateSpeedDegPerSec = 30f; // �ʴ� ȸ�� ����
    [SerializeField] private float pushStrength = 1f;          // ǥ�� �ӵ����� ������ �󸶳� ������ ��������

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Kinematic ����
        //rb.isKinematic = true;
        //rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        // Kinematic ��ü�̹Ƿ� transform.Rotate()�� rb.MoveRotation()���� ���� ȸ��
        // ���⼭�� X�� ȸ���� ���÷�, �ʴ� rotateSpeedDegPerSec��ŭ ȸ��
        transform.Rotate(Vector3.up, rotateSpeedDegPerSec * Time.fixedDeltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("In OnCollisionStay");
        // �浹 ��밡 Rigidbody�� ���� �־�� �������� ���� �� �� ����
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb == null) {
            Debug.Log("player collision is null");
            return;
        }
        // �ʴ� ȸ�� ����(deg/s)�� ����(rad/s)���� ��ȯ
        float angularSpeed = rotateSpeedDegPerSec * Mathf.Deg2Rad;
        // �浹 �������� �ܼ� ���
        foreach (ContactPoint contact in collision.contacts)
        {
            // ȸ�� �߽�(���� ��ġ)���κ��� ���������� �Ÿ� (������ r)
            float radius = Vector3.Distance(contact.point, transform.position);

            // ���ӵ�( m/s ) ~= ���ӵ�(rad/s) * ������(m)
            float surfaceLinearSpeed = angularSpeed * radius;

            // ���ϴ� ��(force) = (���� ���) * surfaceLinearSpeed
            // �����δ� �������ӵ� �� ���� ����ؾ� ������, ���⼭�� ����ȭ
            float forceValue = surfaceLinearSpeed * pushStrength;

            // ���� Z�� ����(����)���� forceValue��ŭ ���� ����
            // (ť���� ���� forward ������ ���� ��ȸ���� ���� �и��� ���⡱�� �´��� Ȯ�� �ʿ�)
            otherRb.AddRelativeForce(0f, 0f, (-1)*forceValue, ForceMode.Acceleration);
        }


    }
}
