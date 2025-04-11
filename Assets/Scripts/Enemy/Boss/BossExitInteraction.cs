using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExitInteraction : StateMachineBehaviour
{
    public string triggerName;
    public bool isRanged;
    const string INTERACTING_LABEL = "IsInteracting";
    const string ATTACKING_LABEL = "Attacking";
    private EnemyState enemyState;
    private BossController bossContoller;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyState = animator.GetComponent<EnemyState>();
        bossContoller = animator.GetComponent<BossController>();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        animator.SetBool(INTERACTING_LABEL, false);
        animator.SetBool(ATTACKING_LABEL, false);

        if (enemyState.state == EnemyState.State.Invincible)
        {
            enemyState.state = EnemyState.State.Idle;
        }

        if (bossContoller != null)
        {
            if (!isRanged) bossContoller.AttackCooltime();
            else bossContoller.RangedCooltime();
        }
    }
}