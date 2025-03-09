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
        // �±װ� "Player"�� ������Ʈ�� Ʈ���ſ� ����
        if (other.CompareTag("Player") && !isMoving)
        {
            Debug.Log("player�� Ground thorn ���� �ߵ�");
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

        // �ö� ��ǥ ��ġ(���� ��ġ + �������� raiseHeight��ŭ)
        Vector3 targetPosition = originalPosition + Vector3.up * raiseHeight;

        // 1) ���� ������ ���
        while (Vector3.Distance(groundThorn.position, targetPosition) > 0.01f)
        {
            groundThorn.position = Vector3.MoveTowards(
                groundThorn.position,
                targetPosition,
                raiseSpeed * Time.deltaTime
            );
            yield return null; // �� ������ ���� �ٽ� �ݺ�
        }

        // 2) ��� ����
        yield return new WaitForSeconds(stayTime);

        // 3) �Ʒ��� õõ�� �ϰ�
        while (Vector3.Distance(groundThorn.position, originalPosition) > 0.01f)
        {
            groundThorn.position = Vector3.MoveTowards(
                groundThorn.position,
                originalPosition,
                fallSpeed * Time.deltaTime
            );
            yield return null;
        }

        // ������ �Ϸ�
        isMoving = false;
    }
}
