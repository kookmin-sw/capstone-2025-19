using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Arrow : MonoBehaviour
{
    [SerializeField] private List<GameObject> arrows;

    [SerializeField] private float arrowSpeed = 10f;     // ȭ�� �̵� �ӵ�
    [SerializeField] private float arrowLifeTime = 3f;  // ȭ���� �߻�� �� �����Ǵ� �ð�(��)
    [SerializeField] private float fireInterval = 0.1f; // ȭ�� �߻� ����(��)

    public bool triggered = false;

    public IEnumerator FireArrowsRoutine()
    {
        Debug.Log("ȭ�� �߻� �ڷ�ƾ ����");

        List<GameObject> arrowPool = new List<GameObject>(arrows);

        while (arrowPool.Count > 0)
        {

            int randomIndex = Random.Range(0, arrowPool.Count);
            GameObject arrow = arrowPool[randomIndex];

            arrowPool.RemoveAt(randomIndex);

            StartCoroutine(MoveArrowForward(arrow));

            yield return new WaitForSeconds(fireInterval);
        }
    }

    private IEnumerator MoveArrowForward(GameObject arrow)
    {
        float timer = 0f;
        while (timer < arrowLifeTime)
        {
            //arrow.transform.Translate(Vector3.forward * arrowSpeed * Time.deltaTime, Space.World);
            arrow.transform.Translate(Vector3.right * arrowSpeed * Time.deltaTime, Space.World);
            timer += Time.deltaTime;
            yield return null;
        }

        // �߻�� �� �����ð� ������ ȭ�� ��Ȱ��ȭ
        arrow.SetActive(false);
    }
}
