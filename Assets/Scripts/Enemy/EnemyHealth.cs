using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    private Animator animator;
    private EnemyState enemyState;
    public Slider hpBar;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyState = GetComponent<EnemyState>();
    }
    void UpdateHpBar()
    {
        hpBar.value = currentHealth / maxHealth;
    }

    public void TakeDamage(float damage, DamageCollider attackerWeapon, Vector3 contactPos, ParticleSystem hitEffect)
    {
        DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
        
        #region Escape
        // when Enemy is invincible
        if (enemyState.state == EnemyState.State.Invincible) return;

        if (attackerWeapon != null && myWeaponCollider !=null)
        {
            if (animator.GetBool("Attacking") &&(myWeaponCollider.tenacity > attackerWeapon.tenacity) ) return;
        }
        #endregion

        #region Hit
        currentHealth -= damage;
        UpdateHpBar();

        if (myWeaponCollider != null)
        {
            myWeaponCollider.dontOpenCollider = true;
            if (myWeaponCollider.damageCollider.enabled) myWeaponCollider.UnableDamageCollider();
        }

        if (hitEffect != null)
        {
            StartCoroutine(WaitForParticleEnd(hitEffect, contactPos));
        }
    
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == "Hit")
            {
                animator.SetTrigger("Hit");
                enemyState.state = EnemyState.State.Invincible;
                break;
            }
        }
        #endregion

        // die
        if (currentHealth <= 0)
        {
            animator.ResetTrigger("Hit");
            currentHealth = 0;
            animator.SetTrigger("Die");
            GetComponent<EnemyController>().DeathTrigger();
        }
    }

    IEnumerator WaitForParticleEnd(ParticleSystem particle, Vector3 position)
    {
        ParticleSystem ps = Instantiate(particle, position, Quaternion.identity);
        ps.Play(); 

        while (ps.IsAlive(true))
        {
            yield return null;
        }

        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

}
