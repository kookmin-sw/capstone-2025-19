using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAH : MonoBehaviour
{
    private Animator animator;

    public enum AnimParam
    {
        Stop,
        Following,
        Attacking,
        Die,
        Hit
    }

    private readonly Dictionary<AnimParam, int> animParamIDs = new();

    private void Awake()
    {
        animator = GetComponent<Animator>();

        animParamIDs[AnimParam.Stop] = Animator.StringToHash("Stop");
        animParamIDs[AnimParam.Following] = Animator.StringToHash("Following");
        animParamIDs[AnimParam.Attacking] = Animator.StringToHash("Attacking");
        animParamIDs[AnimParam.Hit] = Animator.StringToHash("Hit");
        animParamIDs[AnimParam.Die] = Animator.StringToHash("Die");
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

    public Animator GetAnimator()
    {
        return animator;
    }

    public void RootMotion(bool value)
    {
        animator.applyRootMotion = value;
    }
}
