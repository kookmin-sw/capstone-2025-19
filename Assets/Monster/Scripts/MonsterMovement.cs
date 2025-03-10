using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [SerializeField] Animator animator;

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

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        switch (_state)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
        }
    }

    public void UpdateIdle()
    {

    }

    public void FindPlayer()
    {
        animator.SetBool("FindPlayer", true);
    }

    public void MissingPlayer()
    {
        animator.SetBool("FindPlayer", false);
    }

    

}
