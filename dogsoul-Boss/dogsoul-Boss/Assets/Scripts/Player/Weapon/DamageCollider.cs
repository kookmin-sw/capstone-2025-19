using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using PlayerControl;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public Collider damageCollider;
    public GameObject trailRenderer;

    public float damage;
    public float tenacity;
    public ParticleSystem hitEffect;
    public bool dontOpenCollider = false;
    public bool isRanged;

    void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        if (!dontOpenCollider)
        {
            damageCollider.enabled = true;
            if (trailRenderer != null) trailRenderer.SetActive(true);
        }
        else
        {
            dontOpenCollider = false;
        }
    }

    public void UnableDamageCollider()
    {
        damageCollider.enabled = false;
        dontOpenCollider = false;
        if (trailRenderer != null) trailRenderer.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (tag == "PlayerWeapon" && other.tag == "Enemy")
        {
            other.GetComponent<EnemyHealth>().TakeDamage(damage, this, hitEffect);
        }

        else if (tag =="EnemyWeapon" && other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage, this, hitEffect, false);
        }
    }
}
