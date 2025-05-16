using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BrokenEffect : MonoBehaviour
{
    [SerializeField] protected GameObject BrokenObject;
    [SerializeField] protected GameObject FixedObject;
    [SerializeField] protected float removeTime = 5.0f;
    [HideInInspector] public bool isBroken = false;

    protected void Start()
    {
        BrokenObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerWeapon") || other.CompareTag("Enemy"))
        {
            ActiveTrigger();
        }
    }

    protected virtual void ActiveTrigger()
    {
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
        isBroken = true;
        StartCoroutine(removeTimer());
    }

    protected virtual IEnumerator removeTimer()
    {
        yield return new WaitForSeconds(removeTime);
        
        Destroy(gameObject);
    }
}
