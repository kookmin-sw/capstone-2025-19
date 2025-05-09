using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BrokenEffect : MonoBehaviour
{
    [SerializeField] protected GameObject BrokenObject;
    [SerializeField] protected GameObject FixedObject;
    [SerializeField] protected float removeTime = 5.0f;

    protected void Start()
    {
        BrokenObject.SetActive(false);
    }

    protected void OnTriggerEnter(Collider other)
    {
        Debug.Log("Broken Trigger On");
        if(other.CompareTag("PlayerWeapon") || other.CompareTag("Enemy"))
        {
            ActiveTrigger();
        }
    }

    protected virtual void ActiveTrigger()
    {
        BrokenObject.SetActive(true);
        FixedObject.SetActive(false);
        StartCoroutine(removeTimer());
    }

    protected virtual IEnumerator removeTimer()
    {
        yield return new WaitForSeconds(removeTime);
        
        Destroy(gameObject);
    }
}
