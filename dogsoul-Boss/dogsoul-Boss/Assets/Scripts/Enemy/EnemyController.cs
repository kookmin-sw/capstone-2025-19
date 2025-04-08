using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


[RequireComponent(typeof(EnemyState))]
public class EnemyController : MonoBehaviour
{    
    [SerializeField] Transform target;

    [Header("Basic Settings")]
    [SerializeField] float basicSpeed;
    [SerializeField] float attackDistance;
    [SerializeField] float chaseDistance = 10f;
    [SerializeField] int maxPatterns = 1;

    [Header("Patrol Settings")]
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] float waypointDwellingTime = 2f;
    [SerializeField] [Range(0.2f, 10)] float waypointTolerance = 2f;

    Animator animator;
    Vector3 spawnPosition;
    NavMeshAgent agent;
    NavMeshPath path;
    float distance = Mathf.Infinity;
    int waypointIndex;
    private float timeSinceArrivedWaypoint = 0;
    private int attackPatternNo = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    private void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        print("monster");
        if (animator.GetBool("IsInteracting")) return;

        float distanceToPlayer = CalculDistance();

        if (distanceToPlayer <= attackDistance)
        {
            Attack();
        }
        else if (distanceToPlayer <= chaseDistance && distanceToPlayer > attackDistance)
        {
            Chase();
        }
        else
        {
            if (patrolPath != null)
            {
                Patrol();
            }
            else
            {
                if (Vector3.Distance(transform.position, spawnPosition) < 1f)
                {
                    Idle();
                }
                else
                {
                    BackToSpawnPosition();
                }
            }
        }
        timeSinceArrivedWaypoint += Time.deltaTime;
        if (!animator.GetBool("Attacking"))
        {
            agent.isStopped = false;
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

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(target.position);

        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);
    }

    private void Attack()
    {
        transform.LookAt(target);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Following", false);
        attackPatternNo = Random.Range(0, maxPatterns);
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackPatternNo", attackPatternNo);
        animator.SetBool("Attacking", true);
    }

    private void Idle()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        animator.SetBool("Stop", true);
        animator.SetBool("Following", false);
    }

    private void Patrol()
    {
        Vector3 destination = spawnPosition;

        if(patrolPath != null)
        {
            if (AtWaypoint())
            {
                if (timeSinceArrivedWaypoint > waypointDwellingTime)
                {
                    CycleWaypoint();
                }
            }
            else
            {
                agent.isStopped = false;
                timeSinceArrivedWaypoint = 0;
            }
            destination = GetCurrrentWaypoint();
        }

        animator.SetBool("Stop", false);
        animator.SetBool("Following", true);

        agent.SetDestination(destination);
    }

    #region PatrolUtils
    private Vector3 GetCurrrentWaypoint()
    {
        return patrolPath.GetChildPosition(waypointIndex);
    }

    private void CycleWaypoint()
    {
        waypointIndex = patrolPath.GetNextIndex(waypointIndex);
    }

    private bool AtWaypoint()
    {
        agent.isStopped = true;
        float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrrentWaypoint());
        return distanceToWaypoint < waypointTolerance;
    }
    #endregion
}