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

    // ���� ���ݸ��� KnifeDropRoutine�� ȣ�����ִ� �ڷ�ƾ
    private IEnumerator KnifeDropByInterval()
    {
        while (true)
        {
            // IntervalTime��ŭ ��� ��
            yield return new WaitForSeconds(IntervalTime);

            // �̹� �����̰� ���� �ʴٸ� KnifeDropRoutine ����
            if (!isMoving)
            {
                StartCoroutine(KnifeDropRoutine());
            }
        }
    }

    private IEnumerator KnifeDropRoutine()
    {
        isMoving = true;

        // ������ ��ǥ ��ġ(���� ��ġ + �������� raiseHeight��ŭ)
        Vector3 targetPosition = originalPosition + Vector3.down * fallHeight;

        // 1) �Ʒ��� ������ �ϰ�
        while (Vector3.Distance(knife.transform.position, targetPosition) > 0.01f)
        {
            knife.transform.position = Vector3.MoveTowards(
                knife.transform.position,
                targetPosition,
                fallSpeed * Time.deltaTime
            );
            yield return null; // �� ������ ���� �ٽ� �ݺ�
        }

        // 2) ��� ����
        yield return new WaitForSeconds(stayTime);

        // 3) ���� õõ�� ���
        while (Vector3.Distance(knife.transform.position, originalPosition) > 0.01f)
        {
            knife.transform.position = Vector3.MoveTowards(
                knife.transform.position,
                originalPosition,
                upSpeed * Time.deltaTime
            );
            yield return null;
        }

        // ������ �Ϸ�
        isMoving = false;
    }
}
