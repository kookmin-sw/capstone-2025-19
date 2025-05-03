using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocomotion : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float minimumDetectAngle = -50f;
    public float maximumDetectAngle = 50f;
    public LayerMask detectionLayer;

    [Header("Debug")]
    public Health currentTarget;

    // Start is called before the first frame update
    public void HandleDetection()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        for (int i=0; i<colliders.Length; i++)
        {
            print(colliders[i].name);
            Health player = colliders[i].transform.GetComponent<Health>();
            if (player != null)
            {
                Vector3 targetDirection = player.transform.position - transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                if (viewableAngle > minimumDetectAngle && viewableAngle < maximumDetectAngle)
                {
                    currentTarget = player;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
