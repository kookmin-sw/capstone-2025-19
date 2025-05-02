using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAttack : MonoBehaviour
{
    Transform target;
    [SerializeField] float attackDamage;

    private void Update()
    {
        //target = GetComponentInParent<EnemyController>().target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            //target.GetComponent<PlayerHealth>().TakeDamage(attackDamage, null, Vector3.zero, null, false);
        }
    }
}
