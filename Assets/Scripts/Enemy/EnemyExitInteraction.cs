using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExitInteraction : StateMachineBehaviour
{
    public string triggerName;
    const string INTERACTING_LABEL = "IsInteracting";
    const string ATTACKING_LABEL = "Attacking";
    private EnemyState enemyState;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyState = animator.GetComponent<EnemyState>();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        animator.SetBool(INTERACTING_LABEL, false);
        animator.SetBool(ATTACKING_LABEL, false);
        DamageCollider damageCollider = animator.GetComponentInChildren<DamageCollider>();
        if (damageCollider != null) damageCollider.UnableDamageCollider();

        if (enemyState.state == EnemyState.State.Invincible)
        {
            enemyState.state = EnemyState.State.Idle;
        }
    }
}