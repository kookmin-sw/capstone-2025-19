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

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "EnemyWeapon")
        {
            #region Escape
            // when player is invincible
            if (PlayerState.Instance.state == PlayerState.State.Invincible)
            {
                print("Invincible");
                return;
            }

            DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
            DamageCollider opponentWeaponCollider = collision.GetComponent<DamageCollider>();

            // when my weapon is more heavier
            if (animationHandler.GetBool(AnimationHandler.AnimParam.Attacking) && myWeaponCollider.tenacity > opponentWeaponCollider.tenacity)
            {
                print("Tenacity wins");
                return;
            }
            #endregion

            #region Hit
            currentHealth -= opponentWeaponCollider.damage;
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
                animationHandler.SetTrigger(AnimationHandler.AnimParam.Die);
            }

        }
    }

}
