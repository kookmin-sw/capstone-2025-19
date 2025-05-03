using System.Collections;
using UnityEngine;

public class FallingTile : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float delayBeforeFall = 2f;   // Ÿ�� �������� �� ��� �ð�
    [SerializeField] private float timeToVanish = 10f;    // ������ �� ���������� �ð�

    private Rigidbody rb;
    private bool hasTriggered = false;

    private void Start()
    {
        // Ÿ�Ͽ� �پ� �ִ� Rigidbody ��������
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �÷��̾ ����� �� �� ���� �����ϵ���
        if (!hasTriggered && collision.gameObject.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        // 2�� ���
        yield return new WaitForSeconds(delayBeforeFall);

        // �߷¿� ���� ��������
        rb.isKinematic = false;

        // �����̳� ��鸲 ȿ��

        //10�� �� Ÿ�� ��Ȱ��ȭ
        yield return new WaitForSeconds(timeToVanish);
        gameObject.SetActive(false);
    }
}
