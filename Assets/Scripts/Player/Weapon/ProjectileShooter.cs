using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;

    LockOnTarget nearestTarget;
    private string targetTag;
    // Start is called before the first frame update
    public void Shoot()
    {
        nearestTarget = FindNearestTarget();

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.tag = transform.root.tag == "Player" ? "Player" : "Enemy";
        targetTag = projectile.tag == "Player" ? "Enemy" : "Player";

        if (nearestTarget != null)
        {
            projectile.transform.LookAt(nearestTarget.lockOnTarget.transform);
        }
        else
        {
            print("Coudnt find target");
            projectile.transform.Rotate(transform.root.forward);
        }
    }

    private LockOnTarget FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.root.position, 10f);
        LockOnTarget nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestEnemy = collider.GetComponent<LockOnTarget>();
                }
            }
        }
        return nearestEnemy;
    }
}
