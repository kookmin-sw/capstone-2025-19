using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonShot : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletLifetime = 1.35f;  //is Equal to bullet animation time
    [SerializeField] float shotInterval = 6.0f; 

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shotInterval)
        {
            timer = 0f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Destroy(bullet, bulletLifetime);
        }
    }
}
