using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;

    Vector3 spawnPosition;

    NavMeshAgent agent;
    NavMeshPath path;
    EnemyLocomotion enemyLocomotion;
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
        Hit,
        BackToSpawn
    }

    [SerializeField] List<string> attackList = new List<string>{ "AttackDownward", "ComboAttack1"};
    //[SerializeField] List<float> attackDistanceList = new List<float> {5f, 2f};
    //List<float> attackAniInitRotate = new List<float> {19f, 35f };

    float attackDistance = 2f;
    string nextAttackMotion;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyLocomotion = GetComponent<EnemyLocomotion>();
        path = new NavMeshPath();
    }

    private void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        if (_state == MonsterState.Hit)
        {
            return;
        }

        enemyLocomotion.HandleDetection();

        float distanceToPlayer = CalculDistance();

        if (distanceToPlayer <= attackDistance)
        {
            _state = MonsterState.Attack;
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            _state = MonsterState.Following;
        }
        else
        {
            if (Vector3.Distance(transform.position, spawnPosition) < 1f) _state = MonsterState.Idle;
            else _state = MonsterState.BackToSpawn;
        }

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
        if (agent.CalculatePath(target.position, path))
        {
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
        agent.isStopped = false;
        agent.SetDestination(spawnPosition);

        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateFollowing()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void UpdateAttack()
    {
        transform.LookAt(target);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Following", false);
        chooseAttackMotion();
        animator.SetTrigger(nextAttackMotion);
    }

    public void UpdateIdle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
    }

    public void AttackTriggerReset()
    {
        attackList.ForEach(attack => 
        {
            animator.ResetTrigger(attack);
        });
    }
    public void OnHitEnd()
    {
        _state = MonsterState.Idle;

        animator.ResetTrigger("Hit");
    }

    public void OnJumpAttckEnd()
    {
        _state = MonsterState.Following;

        animator.ResetTrigger("JumpAttack");
    }

    public void OnDownAttackEnd()
    {
        _state = MonsterState.Following;

        animator.ResetTrigger("AttackDownward");
    }

        private void chooseAttackMotion()
    {
        int randomAttackIndex = Random.Range(0, attackList.Count);

        nextAttackMotion = attackList[randomAttackIndex];
    }
}