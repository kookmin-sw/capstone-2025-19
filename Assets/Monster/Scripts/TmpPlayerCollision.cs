using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpPlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("몬스터한테 맞음");
        }
    }
}
