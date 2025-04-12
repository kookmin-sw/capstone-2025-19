using System.Collections;
using System.Collections.Generic;
using PlayerControl;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    float detectionRadius = 15f;
    LayerMask detectionLayer;

    void Awake()
    {
        detectionLayer = 1 << LayerMask.NameToLayer("Player");
    }

    public Transform GetClosestPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestTarget = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerController player = colliders[i].GetComponent<PlayerController>();
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    closestTarget = player.transform;
                }
            }
        }

        return closestTarget;
    }
    
}
