using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitInteraction : StateMachineBehaviour
{
    public string triggerName;
    public float freeTime = 0.75f;
    const string INTERACTING_LABEL = "Interacting";
    const string BLOCKING_LABEL = "Blocking";

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float progress = stateInfo.normalizedTime;
        if (progress >= freeTime && progress < 1.0f)
        {      
            animator.SetBool(INTERACTING_LABEL, false);
            animator.applyRootMotion = false;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        animator.SetBool(BLOCKING_LABEL, false);
    }
}
