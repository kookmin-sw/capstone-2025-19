using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LongMonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] float attackDistance = 15f;   // ���� ���� �Ÿ�
    //[SerializeField] GameObject tempCollisionObject; //���Ͱ� ���� ���⸦ ��� �� �ӽ� ���.

    Vector3 spawnPosition;

    NavMeshAgent agent;
    NavMeshPath path;
    float distance = Mathf.Infinity;

    MonsterState _state = MonsterState.Idle;

    public enum MonsterState
    {
        Die,
        Moving,
        Following,
        Jumping,
        Falling,
        Idle,
        Attack,
        Reaction,
        BackToSpawn
    }

    [SerializeField] List<string> attackList = new List<string>{ "Shot",};
    //[SerializeField] List<float> attackDistanceList = new List<float> {5f, 2f};
    //List<float> attackAniInitRotate = new List<float> {19f, 35f };


    private string nextAttackMotion;
    private void chooseAttackMotion()
    {
        /*
        //target���� �Ÿ��� ������� ���� ����
        float distanceToPlayer = CalculDistance();
        if (distanceToPlayer >= 5f)
        {
            // ���� ����
            nextAttackMotion = "JumpAttack";
            attackDistance = 5f;
        }
        else
        {
            // ���� ����
            nextAttackMotion = "AttackDownward";
            attackDistance = 2f;
        }*/
        int randomAttackIndex = Random.Range(0, attackList.Count);

        nextAttackMotion = attackList[randomAttackIndex];
        //nextAttackMotion = "ComboAttack1";
        //attackDistance = 2f;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        agent.stoppingDistance = 2f;
    }

    private void Start()
    {
        //tempCollisionObject.SetActive(false);

        //���� ���� ���� ����
        spawnPosition = transform.position;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            return;
        }
        //Reaction update�� �ƿ� ���⼭ ó��

        _state = MonsterState.Reaction;
        // Reaction �ִϸ��̼� ��� or Ʈ����
        animator.SetTrigger("Hit");
        // Reaction ���� ���� (��: ����, ��� ��)
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //������ �Ϸ� �ߴ� Animation Ʈ���� ����
        AttackTriggerReset();
    }

    void Update()
    {
        //Debug.Log(_state);
        if (_state == MonsterState.Reaction)
        {
            return;
        }

        //�÷��̾���� �Ÿ��� Ȯ��
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        //float distanceToPlayer = CalculDistance();
        //Debug.Log($"���� �Ÿ� : {distanceToPlayer}");

        // 1) ���� ��Ÿ� �̳�����?
        if (distanceToPlayer <= attackDistance)
        {
            if (HasLineOfSight(target)) //ĳ���Ͱ� ���̸�
            {
                animator.SetBool("SeeTarget", true);
                animator.SetBool("Follow", false);
                Debug.Log("ĳ���� ����");
                _state = MonsterState.Attack;
            }
            else
            {
                animator.SetBool("SeeTarget", false);
                _state = MonsterState.Following;
            }
            
        }
        else
        {
            animator.SetBool("SeeTarget", false);
            //if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) _state = MonsterState.Idle;
            //else
            //{

            //    if (_state != MonsterState.BackToSpawn)
            //    {
            //        Debug.Log("�̰� ����Ǹ� �ȵǴ� �ǵ�?");
            //        animator.SetTrigger("BackToPosition");
            //    }
            //    _state = MonsterState.BackToSpawn;
            //}
            if (_state != MonsterState.BackToSpawn)
            {
                animator.SetTrigger("BackToPosition");
                _state = MonsterState.BackToSpawn;
            }
        }

        // ���º� ����
        switch (_state)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Following:
                UpdateFollowing();
                break;
            case MonsterState.Attack:
                UpdateAttack();
                break;
            case MonsterState.BackToSpawn:
                BackToSpawnPosition();
                break;
        }
    }

    private float CalculDistance()
    {
        // ��� ���
        if (agent.CalculatePath(target.position, path))
        {
            // �ڳ� �� �Ÿ��� ���ؼ� �� ��� ���� ���
            distance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
        }

        return distance;
    }

    private void BackToSpawnPosition()
    {
        // �̵� ����
        agent.isStopped = false;
        agent.SetDestination(spawnPosition);

        //�ִϸ��̼�
        animator.SetBool("Follow", true);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // ��ǥ ���� ���� �� ó��
            // => Idle ���� ��ȯ, �̵� �ִϸ��̼� ���� ��
            _state = MonsterState.Idle;
            animator.SetBool("Follow", false);

            // Ʈ���Ÿ� �� ���� ����ϰ� �ʹٸ�, ���⼭ ResetTrigger �ص� ��
            animator.ResetTrigger("BackToPosition");
        }
    }

    private void UpdateFollowing()
    {
        // �̵� ����
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //�ִϸ��̼�
        animator.SetBool("Follow", true);
    }

    private void UpdateAttack()
    {
        // ���� �ÿ��� ���߰� �ִϸ��̼� ����
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //animator.SetTrigger("Attack");

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f; // �������� ����
        transform.rotation = Quaternion.LookRotation(dir);
        
    }

    public void UpdateIdle()
    {
        //������
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //�ִϸ��̼�
        animator.SetBool("Follow", false);
        animator.ResetTrigger("BackToPosition");
    }



    private bool HasLineOfSight(Transform target)
    {
        Vector3 startPos = transform.position + Vector3.up * 1.5f;
        Vector3 targetPos = target.position + Vector3.up * 1.5f;
        Vector3 dir = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        // ����ĳ��Ʈ �˻�
        // LayerMask�� �������� �� ������, ��ֹ��� �� ���̾ üũ�� �� �ֵ��� ���ִ� ���� �����ϴ�.
        // ��: if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance, obstacleLayerMask))
        if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //����ĳ��Ʈ�� �ƹ��͵� ������ ���� ���
        return false;
    }





    //�ִϸ��̼� ���� �� ���� �޼����

    public void AttackTriggerReset()
    {
        attackList.ForEach(attack => 
        {
            animator.ResetTrigger(attack);
        });
        chooseAttackMotion();
    }

    public void WatchPlayer()
    {
        // Ÿ�� �ٶ󺸱�
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f; // �������� ����
        transform.rotation = Quaternion.LookRotation(dir);

        Debug.Log("WatchPlayer");
    }

    public void AttackEnd()
    {
        Debug.Log("bool SeeTarget set false");
        animator.SetBool("SeeTarget", false);
    }

    public void OnReactionEnd()
    {
        // Reaction �ִϸ��̼� ��
        _state = MonsterState.Idle;

        animator.ResetTrigger("Hit");

        Debug.Log("OnReactionEnd ȣ��");
    }


    public void ColliderOn()
    {
        //���� ��ü�� collider �ѱ�
        //tempCollisionObject.SetActive(true);
        Debug.Log("Collider ����");
    }

    public void ColliderOff()
    {
        //���� ��ü�� collider �ѱ�
        //tempCollisionObject.SetActive(false);
        Debug.Log("Collider ����");
    }

}

// Ÿ�� �ٶ󺸱�
//Vector3 dir = (target.position - transform.position).normalized;
//dir.y = 0f; // �������� ����
//transform.rotation = Quaternion.LookRotation(dir);

////�� ���� �ִϸ��̼� ���� �ʱ� ȸ���� ����
//int attackIndex = attackList.IndexOf(nextAttackMotion);
//if (attackIndex >= 0)
//{
//    //�߰� ȸ�� ��
//    float initRotate = attackAniInitRotate[attackIndex];

//    //transform�� �߰� ȸ���� ��
//    transform.rotation *= Quaternion.Euler(0f, initRotate, 0f);
//}
