using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemRock : MonoBehaviour
{
    Transform target;
    void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        if (target != null) StartCoroutine(Rock());
    }

    IEnumerator Rock()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;

        float jumpHeight = 2f;
        float duration = 1.1f;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(Mathf.PI * t) * jumpHeight;

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
            Destroy(gameObject);
        }
    }
}
