using UnityEngine;
using System;

public class VillagePlayerMove : MonoBehaviour
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
        UpdateAnimator();
        Move();
    }

    private void Move()
    {
        float horizontal = 0f;
        float vertical = 0f;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory) { PlayerState.Instance.ChangeState(PlayerState.State.Idle); }
            else if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Idle) { PlayerState.Instance.ChangeState(PlayerState.State.Inventory); }
        }
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory) { return; }
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

        rb.velocity = moveDirection * moveSpeed;
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = rb.velocity;
        float forwardSpeed = transform.InverseTransformDirection(velocity).z;
        anim.SetFloat("forwardSpeed", forwardSpeed);
    }

}
