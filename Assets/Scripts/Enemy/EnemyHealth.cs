using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator animator;
    private EnemyState enemyState;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyState = GetComponent<EnemyState>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "PlayerWeapon")
        {
            #region Escape
            // when Enemy is invincible
            if (enemyState.state == EnemyState.State.Invincible)
            {
                //print("Invincible");
                return;
            }

            DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
            DamageCollider opponentWeaponCollider = collision.GetComponent<DamageCollider>();

            if (myWeaponCollider == null) return;

            if (animator.GetBool("Attacking"))
            {
                // when my weapon is more heavier
                if (myWeaponCollider.tenacity > opponentWeaponCollider.tenacity)
                {
                    //print("Tenacity wins");
                    return;
                }

                else
                {
                    myWeaponCollider.dontOpenCollider = true;
                    if (myWeaponCollider.damageCollider.enabled)
                    {
                        myWeaponCollider.UnableDamageCollider();
                    }
                }
            }
            #endregion

            #region Hit
            currentHealth -= opponentWeaponCollider.damage;
            animator.SetTrigger("Hit");
            enemyState.state = EnemyState.State.Invincible;
            #endregion

            // die
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.SetTrigger("Die");
            }

        }
    }
}
