using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator animator;
    private MonsterMovement enemy;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<MonsterMovement>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "PlayerWeapon")
        {
            #region Escape
            // when Enemy is invincible
            if (enemy._state == MonsterMovement.MonsterState.Invincible)
            {
                print("Invincible");
                return;
            }

            DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
            DamageCollider opponentWeaponCollider = collision.GetComponent<DamageCollider>();

            // when my weapon is more heavier
            if (enemy._state == MonsterMovement.MonsterState.Attack && myWeaponCollider.tenacity > opponentWeaponCollider.tenacity)
            {
                print("Tenacity wins");
                return;
            }
            #endregion

            #region Hit
            currentHealth -= opponentWeaponCollider.damage;
            animator.SetTrigger("Hit");
            enemy._state = MonsterMovement.MonsterState.Invincible;
            #endregion

            // when attack canceled
            if (myWeaponCollider != null && myWeaponCollider.damageCollider.enabled)
            {
                //print("Collider collapse");
                myWeaponCollider.UnableDamageCollider();
            }

            // die
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.SetTrigger("Die");
            }

        }
    }
}
