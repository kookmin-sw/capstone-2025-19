using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using PlayerControl;
using UnityEngine;

public class PlayerHealth : Health
{
    // Start is called before the first frame update
    AnimationHandler animationHandler;

    void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
        DamageCollider opponentWeaponCollider = attacker.GetComponent<DamageCollider>();

        #region CancelCases
        // #1: when entity is invincible
        if (PlayerState.Instance.state == PlayerState.State.Invincible) return;

        // #2: when my tenacity is larger than attacker's
        if (animationHandler.GetBool(AnimationHandler.AnimParam.Attacking) &&
            myWeaponCollider.tenacity > opponentWeaponCollider.tenacity) return;
        #endregion
        
        #region Hit
        currentHealth -= opponentWeaponCollider.damage;

        myWeaponCollider.dontOpenCollider = true;
        if (myWeaponCollider.damageCollider.enabled) myWeaponCollider.UnableDamageCollider();

        animationHandler.SetTrigger(AnimationHandler.AnimParam.Hit);
        PlayerState.Instance.state = PlayerState.State.Invincible;
        animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
        animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
        #endregion

        // when player is attacked on midair
        PlayerController player = GetComponent<PlayerController>();
        if (player == null) return;
        if (animationHandler.GetBool(AnimationHandler.AnimParam.Jump))
        {
            player.ForceJumpStop();
        }

        // die
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animationHandler.SetTrigger(AnimationHandler.AnimParam.Die);
        }

    }
}
