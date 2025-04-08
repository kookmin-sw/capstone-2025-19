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

    private readonly Dictionary<AnimParam, int> animParamIDs = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();

        animParamIDs[AnimParam.Speed] = Animator.StringToHash("Speed");
        animParamIDs[AnimParam.Grounded] = Animator.StringToHash("Grounded");
        animParamIDs[AnimParam.Jump] = Animator.StringToHash("Jump");
        animParamIDs[AnimParam.FreeFall] = Animator.StringToHash("FreeFall");
        animParamIDs[AnimParam.MotionSpeed] = Animator.StringToHash("MotionSpeed");
        animParamIDs[AnimParam.Attack] = Animator.StringToHash("Attack");
        animParamIDs[AnimParam.RangedAttack] = Animator.StringToHash("RangedAttack");
        animParamIDs[AnimParam.DashAttack] = Animator.StringToHash("DashAttack");
        animParamIDs[AnimParam.Attacking] = Animator.StringToHash("Attacking");
        animParamIDs[AnimParam.Hit] = Animator.StringToHash("Hit");
        animParamIDs[AnimParam.Stun] = Animator.StringToHash("Stun");
        animParamIDs[AnimParam.Die] = Animator.StringToHash("Die");
        animParamIDs[AnimParam.Rolling] = Animator.StringToHash("Rolling");
        animParamIDs[AnimParam.UseItem] = Animator.StringToHash("UseItem");
        animParamIDs[AnimParam.PickUp] = Animator.StringToHash("PickUp");
        animParamIDs[AnimParam.Interacting] = Animator.StringToHash("Interacting");
        animParamIDs[AnimParam.Blocking] = Animator.StringToHash("Blocking");
        animParamIDs[AnimParam.CanDoCombo] = Animator.StringToHash("CanDoCombo");
    }

    public void SetBool(AnimParam param, bool value)
    {
        if (animParamIDs.TryGetValue(param, out int id))
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
        if (animParamIDs.TryGetValue(param, out int id))
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
        if (animParamIDs.TryGetValue(param, out int id))
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
        if (animParamIDs.TryGetValue(param, out int id))
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
        if (animParamIDs.TryGetValue(param, out int id))
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
        if (animParamIDs.TryGetValue(param, out int id))
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

