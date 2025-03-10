using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // 추적 시작 거리
    [SerializeField] private float attackDistance = 2f; // 공격 거리

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
        //플레이어와의 거리를 확인
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // 1) 공격 사거리 이내인지?
        if (distanceToPlayer <= attackDistance)
        {
            _state = MonsterState.Attack;
        }
        // 2) 추적 사거리 이내인지?
        else if (distanceToPlayer <= chaseDistance)
        {
            _state = MonsterState.Following;
        }
        // 3) 그 밖에는 Idle
        else
        {
            _state = MonsterState.Idle;
        }

        Debug.Log(_state);
        // 상태별 동작
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
        // 이동 시작
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //애니메이션
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
        animator.ResetTrigger("JumpAttack");
    }

    private void UpdateAttack()
    {
        // 공격 시에는 멈추고 애니메이션 실행
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Following", false);
        animator.SetTrigger("JumpAttack");
    }

    public void UpdateIdle()
    {
        //움직임
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //애니메이션
        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
        animator.ResetTrigger("JumpAttack");
    }

}
