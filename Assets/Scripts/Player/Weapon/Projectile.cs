using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float duration;
    private float lifeTime = 0f;

    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= duration) Destroy(gameObject);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((tag == "Player" && other.tag == "Enemy") || (tag == "Enemy" && other.tag == "Player"))
            Destroy(gameObject);
    }
}
