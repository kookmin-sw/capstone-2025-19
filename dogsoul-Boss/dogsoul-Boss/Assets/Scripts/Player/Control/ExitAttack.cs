using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAttack : StateMachineBehaviour
{
    public string triggerName;
    [Range(0, 0.99f)]public float freeProgressRate = 0.9f;
    public float ComboStart = 0.3f;
    public float ComboEnd = 0.7f;

    const string CANDOCOMBO_LABEL = "CanDoCombo";
    const string INTERACTING_LABEL = "Interacting";
    const string BLOCKING_LABEL = "Blocking";
    const string ATTACKING_LABEL = "Attacking";

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float progress = stateInfo.normalizedTime;
        if (progress >= freeProgressRate && progress < 1.0f)
        {      
            animator.SetBool(INTERACTING_LABEL, false);
            animator.applyRootMotion = false;
        }
        else
        {
            animator.SetBool(INTERACTING_LABEL, true);
            animator.SetBool(BLOCKING_LABEL, true);
            animator.applyRootMotion = true;
        }

        if (progress >= ComboStart && progress <= ComboEnd)
        {
            animator.SetBool(CANDOCOMBO_LABEL, true);
        }
        else
        {
            animator.SetBool(CANDOCOMBO_LABEL, false);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(triggerName);
        animator.SetBool(BLOCKING_LABEL, false);
        animator.SetBool(INTERACTING_LABEL, false);
        animator.SetBool(ATTACKING_LABEL, false);
        DamageCollider damageCollider = animator.GetComponentInChildren<DamageCollider>();
        if (damageCollider) damageCollider.UnableDamageCollider();
        animator.applyRootMotion = false;
    }
}