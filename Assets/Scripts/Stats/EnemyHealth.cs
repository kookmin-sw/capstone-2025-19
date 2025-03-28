using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "PlayerWeapon")
        {
            currentHealth -= collision.GetComponent<DamageCollider>().damage;

            animator.SetTrigger("Hit");

            // die
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.SetTrigger("Die");
            }

        }
    }
}
