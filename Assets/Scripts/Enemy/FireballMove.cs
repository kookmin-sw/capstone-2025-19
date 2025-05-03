using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireballMove : MonoBehaviour
{
    [SerializeField] private float speed = 2f;   // �̵� �ӵ�
    [SerializeField] private float lifeTime = 5f;    // n�� �� �ڵ� �ı�

    private Rigidbody rb;
    private float elapsed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        elapsed = 0f;
        //Debug.Log("FireBall �߻� OnEnable");
        //rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {

        Vector3 nextPos = transform.position + transform.forward * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPos);

        elapsed += Time.fixedDeltaTime;
        if (elapsed >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
