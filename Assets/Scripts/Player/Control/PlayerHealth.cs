using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using PlayerControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    AnimationHandler animationHandler;
    //public Slider hpBar;

    void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }
    public void TakeDamage(float damage, DamageCollider attackerWeapon, Vector3 contactPos, ParticleSystem hitEffect, bool isStun)
    {
        DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();

        #region CancelCases
        // #1: when entity is invincible
        if (PlayerState.Instance.state == PlayerState.State.Invincible || PlayerState.Instance.state == PlayerState.State.Die) return;

        // #2: when my tenacity is larger than attacker's
        if (attackerWeapon != null && myWeaponCollider != null)
        {
            if (animationHandler.GetBool(AnimationHandler.AnimParam.Attacking) &&
            myWeaponCollider.tenacity > attackerWeapon.tenacity) return;
        }
        #endregion
        
        #region Hit
        PlayerStatusController.Instance.curHp -= damage;

        if (myWeaponCollider != null)
        {
            myWeaponCollider.dontOpenCollider = true;
            if (myWeaponCollider.damageCollider.enabled) myWeaponCollider.UnableDamageCollider();
        }

        if (hitEffect != null)
        {
            StartCoroutine(WaitForParticleEnd(hitEffect, contactPos));
        }
        
        if (!isStun) animationHandler.SetTrigger(AnimationHandler.AnimParam.Hit);
        else animationHandler.SetTrigger(AnimationHandler.AnimParam.Stun);
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
        if (PlayerStatusController.Instance.curHp <= 0)
        {
            animationHandler.ResetTrigger(AnimationHandler.AnimParam.Hit);
            animationHandler.ResetTrigger(AnimationHandler.AnimParam.Stun);
            PlayerStatusController.Instance.curHp = 0;
            animationHandler.SetTrigger(AnimationHandler.AnimParam.Die);
            player.DeathTrigger();
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
