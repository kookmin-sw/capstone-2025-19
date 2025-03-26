using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // ���� ���� �Ÿ�
    [SerializeField] GameObject tempCollisionObject; //���Ͱ� ���� ���⸦ ��� �� �ӽ� ���.

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

    [SerializeField] List<string> attackList = new List<string>{ "AttackDownward", "ComboAttack1"};
    //[SerializeField] List<float> attackDistanceList = new List<float> {5f, 2f};
    //List<float> attackAniInitRotate = new List<float> {19f, 35f };

    float attackDistance = 2f; // ���� �Ÿ�
    string nextAttackMotion = "AttackDownward";
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
    }

    private void Start()
    {
        tempCollisionObject.SetActive(false);

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
        //float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        float distanceToPlayer = CalculDistance();
        //Debug.Log($"���� �Ÿ� : {distanceToPlayer}");

        // 1) ���� ��Ÿ� �̳�����?
        if (distanceToPlayer <= attackDistance)
        {
            _state = MonsterState.Attack;
        }
        // 2) ���� ��Ÿ� �̳�����?
        else if (distanceToPlayer <= chaseDistance)
        {
            _state = MonsterState.Following;
        }
        // 3) �� �ۿ��� Idle
        else
        {
            if (Vector3.Distance(transform.position, spawnPosition) < 1f) _state = MonsterState.Idle;
            else _state = MonsterState.BackToSpawn;
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
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateFollowing()
    {
        // �̵� ����
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //�ִϸ��̼�
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateAttack()
    {
        // ���� �ÿ��� ���߰� �ִϸ��̼� ����
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        

        animator.SetBool("Following", false);
        //animator.SetBool("Stop", true);
        animator.SetTrigger(nextAttackMotion);
    }

    public void UpdateIdle()
    {
        //������
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //�ִϸ��̼�
        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
    }

    

    public void AttackTriggerReset()
    {
        attackList.ForEach(attack => 
        {
            animator.ResetTrigger(attack);
        });
        chooseAttackMotion();
        Debug.Log("attack trigger reset");
        Debug.Log($"���� ���� ���� ��� : {nextAttackMotion}");
    }

    public void WatchPlayer()
    {
        // Ÿ�� �ٶ󺸱�
        //Vector3 dir = (target.position - transform.position).normalized;
        //dir.y = 0f; // �������� ����
        //transform.rotation = Quaternion.LookRotation(dir);
    }
    public void OnReactionEnd()
    {
        // Reaction �ִϸ��̼� ��
        _state = MonsterState.Idle;

        animator.ResetTrigger("Hit");

        Debug.Log("OnReactionEnd ȣ��");
    }

    public void OnJumpAttckEnd()
    {
        _state = MonsterState.Following;

        animator.ResetTrigger("JumpAttack");

        Debug.Log("OnJumpAttckEnd ȣ��");
    }

    public void OnDownAttackEnd()
    {
        _state = MonsterState.Following;

        animator.ResetTrigger("AttackDownward");
        Debug.Log("OnDownAttackEnd ȣ��");
    }

    public void ColliderOn()
    {
        //���� ��ü�� collider �ѱ�
        tempCollisionObject.SetActive(true);
        Debug.Log("Collider ����");
    }

    public void ColliderOff()
    {
        //���� ��ü�� collider �ѱ�
        tempCollisionObject.SetActive(false);
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
