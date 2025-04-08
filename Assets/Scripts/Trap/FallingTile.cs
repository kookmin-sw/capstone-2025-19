using System.Collections;
using UnityEngine;

public class FallingTile : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float delayBeforeFall = 2f;   // 타일 떨어지기 전 대기 시간
    [SerializeField] private float timeToVanish = 10f;    // 떨어진 뒤 사라지기까지 시간

    private Rigidbody rb;
    private bool hasTriggered = false;

    private void Start()
    {
        // 타일에 붙어 있는 Rigidbody 가져오기
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어가 밟았을 때 한 번만 동작하도록
        if (!hasTriggered && collision.gameObject.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        // 2초 대기
        yield return new WaitForSeconds(delayBeforeFall);

        // 중력에 의해 떨어지게
        rb.isKinematic = false;

        // 감속이나 흔들림 효과

        //10초 후 타일 비활성화
        yield return new WaitForSeconds(timeToVanish);
        gameObject.SetActive(false);
    }
}
