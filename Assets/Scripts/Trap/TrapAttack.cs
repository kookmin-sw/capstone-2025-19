using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAttack : MonoBehaviour
{
    public int damage = 15;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage, null, Vector3.zero, null, false);
        }
    }
}
