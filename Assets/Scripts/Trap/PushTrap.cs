using System.Collections;
using UnityEngine;

/// <summary>
/// ���� ���ݸ��� ������ �������� 'Ƣ��Դٰ�' �ٽ� �������� ���ư��� ���� ��ũ��Ʈ.
/// </summary>
public class PushTrap : MonoBehaviour
{
    // ������ �о�ø���(�Ǵ� �̴�) �Ÿ�
    [SerializeField] private float pushDistance = 0.5f;

    // �ø���(�߻�) �ӵ��� �ٽ� ���ƿ���(�ϰ�) �ӵ�
    [SerializeField] private float pushSpeed = 5.0f;
    [SerializeField] private float returnSpeed = 2.0f;

    // �߻� �� ��� �����ϴ� �ð�
    [SerializeField] private float stayTime = 1.0f;

    // �߻縦 �ݺ��� ����
    [SerializeField] private float intervalTime = 3.0f;

    // ������ (��Ȳ�� ���� ���)
    [SerializeField] private int damage = 15;

    // �߻� ������ enum���� ����
    public enum PushDirection
    {
        Up,
        Down,
        Left,
        Right
        // �ʿ��ϸ� Forward, Backward ���� �߰� ����
    }

    // Inspector���� ���� ����
    [SerializeField] private PushDirection direction = PushDirection.Up;

    // ���ο�
    private Vector3 originalPosition;
    private bool isMoving = false;

    private void Start()
    {
        // ���� ��ġ ���
        originalPosition = transform.position;

        // ���� ���ݸ��� PushRoutine�� �����ϴ� �ڷ�ƾ
        StartCoroutine(AutoPushRoutine());
    }

    /// <summary>
    /// �ֱ������� ������ �߻��Ű�� �ڷ�ƾ
    /// </summary>
    private IEnumerator AutoPushRoutine()
    {
        while (true)
        {
            // IntervalTime��ŭ ��� ��
            yield return new WaitForSeconds(intervalTime);

            // �̹� �����̰� ���� �ʴٸ� �߻�
            if (!isMoving)
            {
                StartCoroutine(PushRoutine());
            }
        }
    }

    /// <summary>
    /// ������ �߻�(�ö�) -> ���� -> �ٽ� �����ϴ� ����
    /// </summary>
    private IEnumerator PushRoutine()
    {
        isMoving = true;

        // �߻� ���� ���� ���ϱ�
        Vector3 pushDir = GetPushDirection(direction);

        // �ö� ��ǥ ��ġ(= ���� ��ġ + �߻���� * �Ÿ�)
        Vector3 targetPosition = originalPosition + pushDir * pushDistance;

        // 1) �о�ø�(�Ǵ� �̴� ����)
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                pushSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 2) ��� ����
        yield return new WaitForSeconds(stayTime);

        // 3) ���� ��ġ�� �ǵ��ư�
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                originalPosition,
                returnSpeed * Time.deltaTime
            );
            yield return null;
        }

        // ������ �Ϸ�
        isMoving = false;
    }

    /// <summary>
    /// enum�� ���� ���� ���� ���� ���͸� ��ȯ
    /// </summary>
    private Vector3 GetPushDirection(PushDirection dir)
    {
        switch (dir)
        {
            case PushDirection.Up:
                return Vector3.up;
            case PushDirection.Down:
                return Vector3.down;
            case PushDirection.Left:
                return Vector3.left;
            case PushDirection.Right:
                return Vector3.right;
            // �ʿ��� ��� Forward, Back, �� �� �߰� ����
            default:
                return Vector3.up; // �⺻��
        }
    }
}
