using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            Debug.Log("Test, playerweapon한테 맞음");
        }
    }
}
