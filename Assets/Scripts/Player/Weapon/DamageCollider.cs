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
    public bool dontOpenCollider = false;

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
            print("Attack Canceled");
            dontOpenCollider = false;
        }
    }

    public void UnableDamageCollider()
    {
        damageCollider.enabled = false;
        dontOpenCollider = false;
        if (trailRenderer != null) trailRenderer.SetActive(false);
    }
}
