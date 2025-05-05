using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BrokenEffect : MonoBehaviour
{
    [SerializeField] GameObject BrokenObject;
    [SerializeField] GameObject FixedObject;

    private void Start()
    {
        BrokenObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Broken Trigger On");
        if(other.CompareTag("PlayerWeapon") || other.CompareTag("Enemy"))
        {
            BrokenObject.SetActive(true);
            FixedObject.SetActive(false);
        }
    }
}
