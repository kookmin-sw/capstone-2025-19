using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // 추적 시작 거리
    
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
        Reaction,
    }

    [SerializeField] List<string> attackList = new List<string>{ "JumpAttack", "AttackDownward"};
    [SerializeField] List<float> attackDistanceList = new List<float> {5f, 2f};
    List<float> attackAniInitRotate = new List<float> {19f, 35f };

    float attackDistance = 5f; // 공격 거리
    string nextAttackMotion = "JumpAttack";
    private void chooseAttackMotion()
    {

        //target과의 거리를 기반으로 공격 선택
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer >= 5f)
        {
            // 점프 공격
            nextAttackMotion = "JumpAttack";
            attackDistance = 5f;
        }
        else
        {
            // 근접 공격
            nextAttackMotion = "AttackDownward";
            attackDistance = 2f;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            return;
        }
        //Reaction update를 아예 여기서 처리

        _state = MonsterState.Reaction;
        // Reaction 애니메이션 재생 or 트리거
        animator.SetTrigger("Hit");
        // Reaction 상태 동작 (예: 경직, 대기 등)
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //이전에 하려 했던 Animation 트리거 리셋
        AttackTriggerReset();
    }

    void Update()
    {
        Debug.Log(_state);

        if(_state == MonsterState.Attack)
        {
            UpdateAttack();
        }
        if (_state == MonsterState.Reaction)
        {
            return;
        }

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
        AttackTriggerReset();
        chooseAttackMotion();
    }

    private void UpdateAttack()
    {
        // 공격 시에는 멈추고 애니메이션 실행
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // 타겟 바라보기
        //Vector3 dir = (target.position - transform.position).normalized;
        //dir.y = 0f; // 수직축은 무시
        //transform.rotation = Quaternion.LookRotation(dir);

        ////각 공격 애니메이션 마다 초기 회전값 보정
        //int attackIndex = attackList.IndexOf(nextAttackMotion);
        //if (attackIndex >= 0)
        //{
        //    //추가 회전 값
        //    float initRotate = attackAniInitRotate[attackIndex];

        //    //transform에 추가 회전을 곱
        //    transform.rotation *= Quaternion.Euler(0f, initRotate, 0f);
        //}

        animator.SetBool("Following", false);
        //animator.SetBool("Stop", true);
        animator.SetTrigger(nextAttackMotion);
        chooseAttackMotion();
    }

    public void UpdateIdle()
    {
        //움직임
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //애니메이션
        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
        AttackTriggerReset();
    }

    

    private void AttackTriggerReset()
    {
        animator.ResetTrigger("JumpAttack");
        animator.ResetTrigger("AttackDownward");

    }

    public void WatchPlayer()
    {
        // 타겟 바라보기
        //Vector3 dir = (target.position - transform.position).normalized;
        //dir.y = 0f; // 수직축은 무시
        //transform.rotation = Quaternion.LookRotation(dir);
    }
    public void OnReactionEnd()
    {
        // Reaction 애니메이션 끝
        _state = MonsterState.Idle;

        animator.ResetTrigger("Hit");

        Debug.Log("OnReactionEnd 호출");
    }

    public void OnJumpAttckEnd()
    {
        _state = MonsterState.Idle;

        animator.ResetTrigger("JumpAttack");

        Debug.Log("OnJumpAttckEnd 호출");
    }

    public void OnDownAttackEnd()
    {
        _state = MonsterState.Idle;

        animator.ResetTrigger("AttackDownward");
        Debug.Log("OnDownAttackEnd 호출");
    }

}
