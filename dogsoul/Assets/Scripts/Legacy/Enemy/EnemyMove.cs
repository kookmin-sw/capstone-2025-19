using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviourPun
{
    NavMeshAgent navMeshAgent;
    GameObject player;

    [SerializeField] float chaseDistance = 5f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float speedRate = 0.3f;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        if (IsTargetInChaseRange()) MoveTo(player.transform.position, speedRate);
        else StopChase();
    }

    void MoveTo(Vector3 pos, float speedRate) // real implement of moving
    {
        navMeshAgent.destination = pos;
        navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedRate);
        navMeshAgent.isStopped = false;
    }

    void StopChase()
    {
        navMeshAgent.destination = transform.position;
    }

    bool IsTargetInChaseRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        return distanceToPlayer < chaseDistance;
    }
}
