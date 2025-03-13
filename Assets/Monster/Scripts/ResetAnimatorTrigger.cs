using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimatorTrigger : StateMachineBehaviour
{
    public string targeTrigger;
    public bool status;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetTrigger(targeTrigger);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(targeTrigger);
    }
}
