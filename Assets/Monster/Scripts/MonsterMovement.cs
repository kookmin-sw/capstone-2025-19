using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // 추적 시작 거리
    [SerializeField] GameObject tempCollisionObject; //몬스터가 실제 무기를 들기 전 임시 사용.

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

    float attackDistance = 2f; // 공격 거리
    string nextAttackMotion = "AttackDownward";
    private void chooseAttackMotion()
    {
        /*
        //target과의 거리를 기반으로 공격 선택
        float distanceToPlayer = CalculDistance();
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

        //몬스터 스폰 지점 저장
        spawnPosition = transform.position;
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
        //Debug.Log(_state);
        if (_state == MonsterState.Reaction)
        {
            return;
        }

        //플레이어와의 거리를 확인
        //float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        float distanceToPlayer = CalculDistance();
        //Debug.Log($"실제 거리 : {distanceToPlayer}");

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
            if (Vector3.Distance(transform.position, spawnPosition) < 1f) _state = MonsterState.Idle;
            else _state = MonsterState.BackToSpawn;
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
            case MonsterState.BackToSpawn:
                BackToSpawnPosition();
                break;
        }
    }

    private float CalculDistance()
    {
        // 경로 계산
        if(agent == null) { agent = GetComponent<NavMeshAgent>(); }
        if (agent.CalculatePath(target.position, path))
        {
            // 코너 간 거리를 더해서 총 경로 길이 계산
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
        // 이동 시작
        agent.isStopped = false;
        agent.SetDestination(spawnPosition);

        //애니메이션
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateFollowing()
    {
        // 이동 시작
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //애니메이션
        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateAttack()
    {
        // 공격 시에는 멈추고 애니메이션 실행
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        

        animator.SetBool("Following", false);
        //animator.SetBool("Stop", true);
        animator.SetTrigger(nextAttackMotion);
    }

    public void UpdateIdle()
    {
        //움직임
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //애니메이션
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
        Debug.Log($"랜덤 선택 공격 모션 : {nextAttackMotion}");
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
        _state = MonsterState.Following;

        animator.ResetTrigger("JumpAttack");

        Debug.Log("OnJumpAttckEnd 호출");
    }

    public void OnDownAttackEnd()
    {
        _state = MonsterState.Following;

        animator.ResetTrigger("AttackDownward");
        Debug.Log("OnDownAttackEnd 호출");
    }

    public void ColliderOn()
    {
        //공격 물체에 collider 켜기
        tempCollisionObject.SetActive(true);
        Debug.Log("Collider 켜짐");
    }

    public void ColliderOff()
    {
        //공격 물체에 collider 켜기
        tempCollisionObject.SetActive(false);
        Debug.Log("Collider 꺼짐");
    }

}

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
