using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyState))]
public class BossController : MonoBehaviour
{
    public Transform target;

    [Header("Basic Settings")]
    [SerializeField] float basicSpeed;
    [SerializeField] float attackDistance;
    [SerializeField] float rangedThresholdDistance;
    [SerializeField] int attackPatterns = 2;
    [SerializeField] int rangedPatterns = 2;

    [Header("Combats")]
    [SerializeField] GameObject rockPrefab;
    [SerializeField] Transform rockInitPos;
    [SerializeField] float jumpDamage = 30f;
    [SerializeField] float rangedCool = 3f;
    [SerializeField] float attackCoolMin = 1f;
    [SerializeField] float attackCoolMax = 2f; // random max


    Animator animator;
    Vector3 spawnPosition;
    EnemyState enemyState;
    NavMeshAgent agent;
    NavMeshPath path;
    DamageCollider damageCollider;

    float distance = Mathf.Infinity;
    private int attackPatternNo = 0;
    private int rangedPatternNo = 0;
    private float attackCoolDelta = 0;
    private float rangedCoolDelta = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        damageCollider = GetComponentInChildren<DamageCollider>();
        enemyState = GetComponent<EnemyState>();
        path = new NavMeshPath();

        agent.speed = basicSpeed;
    }

    private void Start()
    {
        spawnPosition = transform.position;

    }

    void Update()
    {
        if (animator.GetBool("IsInteracting")) return;
        float distanceToPlayer = CalculDistance();

        if (distanceToPlayer < attackDistance)
        {
            if (attackCoolDelta <= 1) 
            {
                Attack();
            }
            else
            {
                Chase();
            }
        } 
        else if (distanceToPlayer > attackDistance && distanceToPlayer < rangedThresholdDistance)
        {
            Chase();
        }
        else if (distanceToPlayer > rangedThresholdDistance)
        {
            if (rangedCoolDelta <= 1)
            {
                RangedAttack();
            }
            else
            {
                Chase();
            }
        }
        if (rangedCoolDelta > 0)
        {
            rangedCoolDelta -= Time.deltaTime;
        }

        if (attackCoolDelta > 0)
        {
            attackCoolDelta -= Time.deltaTime;
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
        attackPatternNo = Random.Range(0, attackPatterns);
        animator.SetTrigger("Attack");
        animator.SetInteger("AttackPatternNo", attackPatternNo);
        animator.SetBool("IsInteracting", true);
        animator.SetBool("Attacking", true);
    }

    #region Attack
    public void EnableAttack()
    {
        damageCollider.EnableDamageCollider();
    }
    public void UnableAttack()
    {
        damageCollider.UnableDamageCollider();
    }
    public void AttackCooltime()
    {
        attackCoolDelta = Random.Range(attackCoolMin, attackCoolMax);
    }
    #endregion

    #region Ranged Attack
    private void RangedAttack()
    {
        transform.LookAt(target);

        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        attackPatternNo = Random.Range(0, attackPatterns);

        animator.SetBool("Following", false);
        animator.SetTrigger("RangedAttack");
        animator.SetBool("Attacking", true);

        rangedPatternNo = Random.Range(0, rangedPatterns);
        animator.SetInteger("RangedPatternNo", rangedPatternNo);
        animator.SetBool("IsInteracting", true);

        
    }
    public void JumpAttack()
    {
        StartCoroutine(JumpCorutine());
    }

    public void ThrowRock()
    {
        if (SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene") { PhotonNetwork.Instantiate($"Prefabs/Enemys/Multiplay/Rock", rockInitPos.position, Quaternion.identity); }
        else { Instantiate(rockPrefab, rockInitPos); }
        
    }
    public void RangedCooltime()
    {
        rangedCoolDelta = rangedCool;
    }

    IEnumerator JumpCorutine()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        float jumpHeight = 5f;
        float duration = 1.1f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            // 포물선 보간
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;

            transform.position = currentPos;

            time += Time.deltaTime;
            yield return null;
        }

        // 착지 처리
        OnLand();

        // 점프 쿨타임
        yield return new WaitForSeconds(3f);
    }

    void OnLand()
    {
        float damageRadius = 10f;
        animator.SetBool("IsInteracting", true);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>().TakeDamage(jumpDamage, null, Vector3.zero, null, true);
                
            }
        }

        // 착지 이펙트, 사운드
        // 예: Instantiate(landingEffect, transform.position, Quaternion.identity);
    }
    #endregion

}