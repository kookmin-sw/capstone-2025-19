using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guillotine_Trigger : MonoBehaviour
{

    public int damage = 15;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player�� �ܵδ뿡 ����");
            /*PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }*/
        }
    }
}
