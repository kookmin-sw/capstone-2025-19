using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Arrow : MonoBehaviour
{
    [SerializeField] private List<GameObject> arrows;

    [SerializeField] private float arrowSpeed = 10f;     // 화살 이동 속도
    [SerializeField] private float arrowLifeTime = 3f;  // 화살이 발사된 후 유지되는 시간(초)
    [SerializeField] private float fireInterval = 0.1f; // 화살 발사 간격(초)

    public bool triggered = false;

    public IEnumerator FireArrowsRoutine()
    {
        Debug.Log("화살 발사 코루틴 실행");

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

        // 발사된 후 일정시간 지나면 화살 비활성화
        arrow.SetActive(false);
    }
}
