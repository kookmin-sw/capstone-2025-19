using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsInteracting : StateMachineBehaviour
{
    private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.SetBool("isInteracting", false);
    }
}
