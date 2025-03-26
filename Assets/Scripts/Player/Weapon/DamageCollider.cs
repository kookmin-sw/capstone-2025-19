using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

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
        if (trailRenderer != null) trailRenderer.SetActive(true);
    }

    public void UnableDamageCollider()
    {
        damageCollider.enabled = false;
        if (trailRenderer != null) trailRenderer.SetActive(false);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (gameObject.tag == "Enemy" && collision.tag == "Player")
        {
            GetHit(collision.GetComponent<Health>());
        }

        if (gameObject.tag == "Player" && collision.tag == "Enemy")
        {
            GetHit(collision.GetComponent<Health>());
        }
    }

    private void GetHit(Health target)
    {
        if (target != null)
        {
            print(target.name + " hit");
            target.TakeDamage(currentWeaponDamage);
        }

    }
}
