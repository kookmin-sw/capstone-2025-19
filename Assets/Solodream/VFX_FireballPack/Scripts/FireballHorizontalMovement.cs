using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace FireballMovement
{
    public class FireballHorizontalMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;     // 원하는 속도
        [SerializeField] private float lifeTime = 6f;       // 몇 초 뒤 리셋/비활성
        [SerializeField] private Transform startingPos;     // Inspector에서 지정

        private Rigidbody rb;
        private VisualEffect vfx;
        private float elapsed;
        private const string Trails = "FireballTrailsActive";

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            vfx = GetComponent<VisualEffect>();

            // 시작 위치 따로 저장하고 싶으면
            if (startingPos == null) startingPos = transform;
        }

        private void OnEnable()       // 발사 직후 또는 재사용 시
        {
            elapsed = 0f;
            vfx.enabled = true;

            // 한 번에 속도 설정
            rb.velocity = transform.forward * moveSpeed;
        }

        private void FixedUpdate()    // 물리 전용
        {
            // 필요하다면 속도 유지(드래그 등으로 느려지는 경우)
            // rb.velocity = transform.forward * moveSpeed;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            // 속도에 따라 이펙트 토글
            vfx.SetBool(Trails, rb.velocity.sqrMagnitude > 1f);

            if (elapsed >= lifeTime)
            {
                ResetProjectile();
            }
        }

        private void ResetProjectile()
        {
            vfx.enabled = false;
            rb.velocity = Vector3.zero;
            transform.position = startingPos.position;
            gameObject.SetActive(false);   // 풀링을 쓴다면
            // Destroy(gameObject);        // 1회용이라면
        }
    }
}