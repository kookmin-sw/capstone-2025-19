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
            PlayerController player = GetComponent<PlayerController>();
            if (player == null) return;
            currentHealth -= collision.GetComponent<DamageCollider>().damage;

            if (animationHandler.GetBool(AnimationHandler.AnimParam.Jump))
            {
                print("dd");
                player.ForceJumpStop();
            }

            animationHandler.SetTrigger(AnimationHandler.AnimParam.Hit);
            animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
            animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);

            // die
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animationHandler.SetTrigger(AnimationHandler.AnimParam.Die);
            }

        }
    }

}
