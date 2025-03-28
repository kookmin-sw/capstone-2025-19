using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using PlayerControl;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;
    public GameObject trailRenderer;

    public float damage = 10;

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
}
