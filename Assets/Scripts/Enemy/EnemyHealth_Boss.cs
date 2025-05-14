using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyHealth_Boss : EnemyHealth
{
    [SerializeField] BossController bossController;
    


    public override void TakeDamage(float damage, DamageCollider attackerWeapon, Vector3 contactPos, ParticleSystem hitEffect)
    {
        DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();

        #region Escape
        // when Enemy is invincible
        if (enemyState.state == EnemyState.State.Invincible || enemyState.state == EnemyState.State.Die) return;

        if (attackerWeapon != null && myWeaponCollider != null)
        {
            if (animator.GetBool("Attacking") && (myWeaponCollider.tenacity > attackerWeapon.tenacity)) return;
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
            
            
        }
    }

    public override void DieActive()
    {
        base.DieActive();
        DungeonGenerator.Instance.CountBossKill();
    }
}
