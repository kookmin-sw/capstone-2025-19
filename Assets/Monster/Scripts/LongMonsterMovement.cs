using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LongMonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] float attackDistance = 15f;   // 추적 시작 거리
    //[SerializeField] GameObject tempCollisionObject; //몬스터가 실제 무기를 들기 전 임시 사용.

    [Header("Weapon")]
    [SerializeField] GameObject ArrowPosition;
    [SerializeField] GameObject Arrow;

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
        int randomAttackIndex = Random.Range(0, attackList.Count);

        nextAttackMotion = attackList[randomAttackIndex];
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
        agent.stoppingDistance = 2f;

        //처음 사격 가능
        animator.SetBool("ShotWait", false);
    }

    private void Start()
    {
        //tempCollisionObject.SetActive(false);

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
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        //float distanceToPlayer = CalculDistance();

        //Is in attackDistance
        if (distanceToPlayer <= attackDistance)
        {
            if (HasLineOfSight(target)) 
            {
                _state = MonsterState.Attack;
            }
            else
            {
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
            //        Debug.Log("이거 실행되면 안되는 건데?");
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
        animator.SetBool("Follow", true);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // 목표 지점 도달 시 처리
            // => Idle 상태 전환, 이동 애니메이션 끄기 등
            _state = MonsterState.Idle;
            animator.SetBool("Follow", false);

            // 트리거를 한 번만 사용하고 싶다면, 여기서 ResetTrigger 해도 됨
            animator.ResetTrigger("BackToPosition");
        }
    }

    private void UpdateFollowing()
    {
        // 이동 시작
        agent.isStopped = false;
        agent.SetDestination(target.position);

        //애니메이션
        animator.SetBool("SeeTarget", false);
        animator.SetBool("Follow", true);
    }

    private void UpdateAttack()
    {
        // 공격 시에는 멈추고 애니메이션 실행
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f; // 수직축은 무시
        transform.rotation = Quaternion.LookRotation(dir);

        animator.SetBool("SeeTarget", true);
        animator.SetBool("Follow", false);
    }

    public void UpdateIdle()
    {
        //움직임
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //애니메이션
        animator.SetBool("Follow", false);
        animator.ResetTrigger("BackToPosition");
    }



    private bool HasLineOfSight(Transform target)
    {
        Vector3 startPos = transform.position + Vector3.up * 1.5f;
        Vector3 targetPos = target.position + Vector3.up * 1.5f;
        Vector3 dir = (targetPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, targetPos);

        // if (Physics.Raycast(startPos, dir, out RaycastHit hit, distance, obstacleLayerMask))
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

        //레이캐스트가 아무것도 맞추지 못한 경우
        return false;
    }





    //애니메이션 끝난 후 적용 메서드들

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
        // 타겟 바라보기
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f; // 수직축은 무시
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
        // Reaction 애니메이션 끝
        _state = MonsterState.Idle;

        animator.ResetTrigger("Hit");

        Debug.Log("OnReactionEnd 호출");
    }


    public void ColliderOn()
    {
        //공격 물체에 collider 켜기
        //tempCollisionObject.SetActive(true);
        Debug.Log("Collider 켜짐");
    }

    public void ColliderOff()
    {
        //공격 물체에 collider 켜기
        //tempCollisionObject.SetActive(false);
        Debug.Log("Collider 꺼짐");
    }

    public void aim_recoilEnd()
    {
        StartCoroutine(WaitAfterShotCoroutine());
    }
    private IEnumerator WaitAfterShotCoroutine()
    {
        Debug.Log("Shot 후 대기 시작");
        animator.SetBool("ShotWait", true);
        agent.isStopped = true;

        float waitTime = Random.Range(3, 6);
        yield return new WaitForSeconds(waitTime);
        animator.SetBool("ShotWait", false);
        agent.isStopped = false;
        Debug.Log("Shot 후 대기 종료, 다음 상태로 전환");
    }

    public void CreateArrow()
    {
        Instantiate(Arrow, ArrowPosition.transform);
    }

    public void LaunchArrow()
    {
        if (ArrowPosition.transform.childCount > 0)
        {
            Transform arrowTransform = ArrowPosition.transform.GetChild(0);
            arrowTransform.SetParent(null);
            ArrowProjectile arrowProj = arrowTransform.GetComponent<ArrowProjectile>();

            Vector3 arrowPos = arrowTransform.position;
            Vector3 targetCenter = target.position + new Vector3(0, 1.0f, 0);

            Vector3 shootDirection = (targetCenter - arrowPos).normalized; //transform.forward;
            arrowProj.SetDirection(shootDirection);
        }
    }

}


