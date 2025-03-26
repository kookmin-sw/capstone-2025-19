using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStatus : MonoBehaviour
{
    public int damage = 10;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Health health = other.GetComponent<Health>();

            health.TakeDamage(damage);
        }
    }
}
