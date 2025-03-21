using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform target;
    [SerializeField] private float chaseDistance = 10f;   // ���� ���� �Ÿ�
    
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

    float attackDistance = 5f; // ���� �Ÿ�
    string nextAttackMotion = "JumpAttack";
    private void chooseAttackMotion()
    {

        //target���� �Ÿ��� ������� ���� ����
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
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
        Debug.Log(_state);

        if(_state == MonsterState.Attack)
        {
            UpdateAttack();
        }
        if (_state == MonsterState.Reaction)
        {
            return;
        }

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
        AttackTriggerReset();
        chooseAttackMotion();
    }

    private void UpdateAttack()
    {
        // ���� �ÿ��� ���߰� �ִϸ��̼� ����
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

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

        animator.SetBool("Following", false);
        //animator.SetBool("Stop", true);
        animator.SetTrigger(nextAttackMotion);
        chooseAttackMotion();
    }

    public void UpdateIdle()
    {
        //������
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        //�ִϸ��̼�
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
        _state = MonsterState.Idle;

        animator.ResetTrigger("JumpAttack");

        Debug.Log("OnJumpAttckEnd ȣ��");
    }

    public void OnDownAttackEnd()
    {
        _state = MonsterState.Idle;

        animator.ResetTrigger("AttackDownward");
        Debug.Log("OnDownAttackEnd ȣ��");
    }

}
