using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertAnimationEnds : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        string currentAnimationName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        Debug.Log("현재 재생 중인 애니메이션: " + currentAnimationName);
    }
}
