using System.Collections;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    public float comboResetTime = 1.5f; 
    private Animator animator;
    private int comboIndex = 0;
    private Coroutine resetCoroutine;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        animator.SetTrigger("Attack");
        animator.SetInteger("ComboIndex", comboIndex);

        if(stateInfo.IsName("Combo1")) { comboIndex=1; } 
        else if(stateInfo.IsName("Combo2")) { comboIndex=2; }
        else comboIndex = 0;

        resetCoroutine = StartCoroutine(ResetCombo());

        print(comboIndex);
    }

    IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboIndex = 0;
        animator.SetInteger("ComboIndex", 0);
    }
}