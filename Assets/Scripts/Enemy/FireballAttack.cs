using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform fireballSpawn;

    public void FireInstantiate()
    {
        if (fireballPrefab == null || fireballSpawn == null) return;

        Instantiate(
            fireballPrefab,
            fireballSpawn.position,
            fireballSpawn.rotation
        );
    }
}
