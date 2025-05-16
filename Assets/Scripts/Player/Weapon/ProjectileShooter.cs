using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject projectilPrefab_M;
    [SerializeField] PhotonView photonView;


    LockOnTarget nearestTarget;

    private string targetTag;
    public float angleToTarget = 60f;

    public void Shoot()
    {
        targetTag = transform.root.tag == "Player" ? "Enemy" : "Player";
        nearestTarget = FindNearestTarget();

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        photonView.RPC(nameof(ShootArrow), RpcTarget.OthersBuffered);
        projectile.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        projectile.GetComponent<DamageCollider>().EnableDamageCollider();

        if (nearestTarget != null)
        {
            //projectile.transform.LookAt(nearestTarget.lockOnTarget.transform);
            projectile.transform.eulerAngles = transform.root.eulerAngles;
            if (projectile.transform.rotation.x < 0)
            {
                projectile.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else
        {
            projectile.transform.eulerAngles = transform.root.eulerAngles;
        }
    }

    private LockOnTarget FindNearestTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
        LockOnTarget nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.tag == targetTag)
            {
                Vector3 dirToTarget = (collider.transform.position - transform.position).normalized;
                float viewAngle = Vector3.Angle(transform.forward, dirToTarget);

                if (viewAngle < angleToTarget)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestEnemy = collider.GetComponent<LockOnTarget>();
                    }
                }
            }
        }
        return nearestEnemy;
    }

    [PunRPC]
    private void ShootArrow()
    {
        GameObject projectile = Instantiate(projectilPrefab_M, transform.position, Quaternion.identity);
        projectile.tag = transform.root.tag == "Player" ? "PlayerWeapon" : "EnemyWeapon";
        if (nearestTarget != null)
        {
            //projectile.transform.LookAt(nearestTarget.lockOnTarget.transform);
            projectile.transform.eulerAngles = transform.root.eulerAngles;
            if (projectile.transform.rotation.x < 0)
            {
                projectile.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else
        {
            projectile.transform.eulerAngles = transform.root.eulerAngles;
        }
    }
}
