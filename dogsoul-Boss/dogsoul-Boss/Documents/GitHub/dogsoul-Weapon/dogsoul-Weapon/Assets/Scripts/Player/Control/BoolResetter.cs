using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolResetter : StateMachineBehaviour
{
    public string boolName;
    public bool status;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(boolName, status);
        Debug.Log("Reset: " + boolName + "as " + status);
    }
}
