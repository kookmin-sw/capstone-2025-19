using UnityEngine;
using Photon.Pun;
using System;

public class PlayerMover : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        UpdateAnimator();
        Move();
    }

    private void Move()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = rb.velocity;
        float forwardSpeed = transform.InverseTransformDirection(velocity).z;
        anim.SetFloat("forwardSpeed", forwardSpeed);
        photonView.RPC("SyncAnimator", RpcTarget.Others, forwardSpeed);
    }

    /*[PunRPC]
    private void SyncAnimator(float forwardSpeed)
    {
        anim.SetFloat("forwardSpeed", forwardSpeed);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }*/
}
