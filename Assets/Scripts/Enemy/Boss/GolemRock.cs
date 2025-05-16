using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRock : MonoBehaviour
{
    Transform target;
    public float damage = 30f;
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        if (target != null) StartCoroutine(Rock());
        
    }

    IEnumerator Rock()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        float height = 2f;
        float speed = 20f;
        
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance / speed; 
        float time = 0f;


        while (time < duration)
        {
            float t = time / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(Mathf.PI * t) * height;

            transform.position = currentPos;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(damage, null, Vector3.zero, null, true);
            Destroy(gameObject);
        }
    }
}
