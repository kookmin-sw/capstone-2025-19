using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // ���� ���� �Ÿ�
    [SerializeField] private float attackDistance = 2f; // ���� �Ÿ�

    NavMeshAgent agent;

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
        Attacked,
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        agent.SetDestination(target.position);
    }

    void Update()
    {
        //�÷��̾���� �Ÿ��� Ȯ��
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

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
            _state = MonsterState.Idle;
        }

        Debug.Log(_state);
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
        }
    }

    private void UpdateFollowing()
    {
        // �̵� ����
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //�ִϸ��̼�
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
        animator.ResetTrigger("JumpAttack");
    }

    private void UpdateAttack()
    {
        // ���� �ÿ��� ���߰� �ִϸ��̼� ����
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Following", false);
        animator.SetTrigger("JumpAttack");
    }

    public void UpdateIdle()
    {
        //������
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //�ִϸ��̼�
        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
        animator.ResetTrigger("JumpAttack");
    }

}
