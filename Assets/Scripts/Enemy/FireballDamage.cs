using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballDamage : MonoBehaviour
{
    [SerializeField] float attackDamage = 10;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(attackDamage, null, Vector3.zero, null, false);
        }
    }
}
