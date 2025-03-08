using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCombat
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        public GameObject trailRenderer;

        public int currentWeaponDamage = 25;

        void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
            trailRenderer.SetActive(true);
        }

        public void UnableDamageCollider()
        {
            damageCollider.enabled = false;
            trailRenderer.SetActive(false);
        }

        void OnTriggerEnter(Collider collision)
        {
            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                
                if (playerStats != null)
                {
                    playerStats.TakeDamage(currentWeaponDamage);
                }
            }

            if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                
                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }

            }
        }
    }
}

