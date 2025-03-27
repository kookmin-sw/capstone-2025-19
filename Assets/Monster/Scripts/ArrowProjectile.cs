using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] GameObject tailTrail;
    [SerializeField] float speed = 20f;      
    [SerializeField] float lifeTime = 5f;    

    private Vector3 direction;

    private void Awake()
    {
        tailTrail.SetActive(false);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        tailTrail.SetActive(true);
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
