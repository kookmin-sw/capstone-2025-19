using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrapMovement : MonoBehaviour
{
    [SerializeField] public Transform dropItemPosition;
    [SerializeField] Collider triggerCollider;

    PhotonView photonView;

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    // ���� ���� ����
    public float jumpForce = 5f;             // ���� ��
    private bool isGrounded = true;          // ���� ��� �ִ��� ����

    // Raycast ����� ���� ���� ���� ����
    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.2f;  // ĳ���� �� �ؿ��� ������� �˻��� �Ÿ�
    [SerializeField] private LayerMask groundMask;              // 'Ground' ���̾�(�Ǵ� �±�)�� �ش��ϴ� ���̾� ����ũ

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        /* 
        if (!photonView.IsMine)
        {
            triggerCollider.enabled = false;
        }
        */
    }

    private void Update()
    {
        // if (!photonView.IsMine) return;

        UpdateAnimator();
        Move();

        // �� üũ(Ȥ�� FixedUpdate���� �ص� ����)
        CheckGround();

        //Debug.Log($"isGrounded : {isGrounded}");
        // �����̽��ٷ� ����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    /// <summary>
    /// �̵� ����
    /// </summary>
    private void Move()
    {
        // Tab Ű�� ���� ��ȯ (Inventory ����/�ݱ�)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
            {
                PlayerState.Instance.ChangeState(PlayerState.State.Idle);
            }
            else if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Idle)
            {
                PlayerState.Instance.ChangeState(PlayerState.State.Inventory);
            }
        }

        // �κ��丮 ���¸� �̵� �Ұ�
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // �̵� �������� ȸ��
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// �ִϸ����� ���¸� �����ϴ� ����
    /// </summary>
    private void UpdateAnimator()
    {
        Vector3 velocity = rb.velocity;
        float forwardSpeed = transform.InverseTransformDirection(velocity).z;

        // �ʿ� �� �ִϸ����Ϳ� �ӵ��� ����
        // anim.SetFloat("forwardSpeed", forwardSpeed);
        // photonView.RPC("SyncAnimator", RpcTarget.Others, forwardSpeed);
    }

    /// <summary>
    /// ���鿡 ��� �ִ��� Raycast�� ����
    /// </summary>
    private void CheckGround()
    {
        // �÷��̾� �ϴܿ��� ���� ���� ���� �ø� �������� �Ʒ� �������� ����ĳ��Ʈ
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;

        // groundCheckDistance �Ÿ� ���� 'groundMask' ���̾ �浹�ϸ� �� ���� �ִٰ� �Ǵ�
        bool hitGround = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);

        isGrounded = hitGround;
    }

    [PunRPC]
    private void SyncAnimator(float forwardSpeed)
    {
        anim.SetFloat("forwardSpeed", forwardSpeed);
    }

    private void FixedUpdate()
    {
        // if (!photonView.IsMine) return;

        // �̵� �ӵ��� ������ٵ� ���� ����
        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    
}
