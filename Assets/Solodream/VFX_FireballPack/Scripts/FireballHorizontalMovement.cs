using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace FireballMovement
{
    public class FireballHorizontalMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;     // ���ϴ� �ӵ�
        [SerializeField] private float lifeTime = 6f;       // �� �� �� ����/��Ȱ��
        [SerializeField] private Transform startingPos;     // Inspector���� ����

        private Rigidbody rb;
        private VisualEffect vfx;
        private float elapsed;
        private const string Trails = "FireballTrailsActive";

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            vfx = GetComponent<VisualEffect>();

            // ���� ��ġ ���� �����ϰ� ������
            if (startingPos == null) startingPos = transform;
        }

        private void OnEnable()       // �߻� ���� �Ǵ� ���� ��
        {
            elapsed = 0f;
            vfx.enabled = true;

            // �� ���� �ӵ� ����
            rb.velocity = transform.forward * moveSpeed;
        }

        private void FixedUpdate()    // ���� ����
        {
            // �ʿ��ϴٸ� �ӵ� ����(�巡�� ������ �������� ���)
            // rb.velocity = transform.forward * moveSpeed;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            // �ӵ��� ���� ����Ʈ ���
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
            gameObject.SetActive(false);   // Ǯ���� ���ٸ�
            // Destroy(gameObject);        // 1ȸ���̶��
        }
    }
}