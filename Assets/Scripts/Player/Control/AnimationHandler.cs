using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private Animator animator;

    public enum AnimParam
    {
        Speed,
        Grounded,
        Jump,
        FreeFall,
        MotionSpeed,
        Attack,
        RangedAttack,
        DashAttack,
        Attacking,
        Hit,
        Stun,
        Die,
        Rolling,
        UseItem,
        PickUp,
        Interacting,
        Blocking,
        CanDoCombo
    }

    private readonly Dictionary<AnimParam, string> animParamIDs = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        print(animator.runtimeAnimatorController.name);

        animParamIDs[AnimParam.Speed] = "Speed";
        animParamIDs[AnimParam.Grounded] = "Grounded";
        animParamIDs[AnimParam.Jump] = "Jump";
        animParamIDs[AnimParam.FreeFall] = "FreeFall";
        animParamIDs[AnimParam.MotionSpeed] = "MotionSpeed";
        animParamIDs[AnimParam.Attack] = "Attack";
        animParamIDs[AnimParam.RangedAttack] = "RangedAttack";
        animParamIDs[AnimParam.DashAttack] = "DashAttack";
        animParamIDs[AnimParam.Attacking] = "Attacking";
        animParamIDs[AnimParam.Hit] = "Hit";
        animParamIDs[AnimParam.Stun] = "Stun";
        animParamIDs[AnimParam.Die] = "Die";
        animParamIDs[AnimParam.Rolling] = "Rolling";
        animParamIDs[AnimParam.UseItem] = "UseItem";
        animParamIDs[AnimParam.PickUp] = "PickUp";
        animParamIDs[AnimParam.Interacting] = "Interacting";
        animParamIDs[AnimParam.Blocking] = "Blocking";
        animParamIDs[AnimParam.CanDoCombo] = "CanDoCombo";

        print(animator.GetBool("Test"));
        print(animator.GetBool(animParamIDs[AnimParam.Attacking]));
    }

    public void SetBool(AnimParam param, bool value)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            animator.SetBool(id, value);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
        }
    }

    public bool GetBool(AnimParam param)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            return animator.GetBool(id);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
            return false;
        }
    }

    public void SetTrigger(AnimParam param)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            animator.SetTrigger(id);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
        }
    }

    public void ResetTrigger(AnimParam param)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            animator.ResetTrigger(id);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
        }
    }

    public void SetFloat(AnimParam param, float value)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            animator.SetFloat(id, value);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
        }
    }

    public void SetInteger(AnimParam param, int value)
    {
        if (animParamIDs.TryGetValue(param, out string id))
        {
            animator.SetInteger(id, value);
        }
        else
        {
            Debug.LogWarning($"Animator Param {param} not found");
        }
    }

    public void RootMotion(bool value)
    {
        animator.applyRootMotion = value;
    }
}

