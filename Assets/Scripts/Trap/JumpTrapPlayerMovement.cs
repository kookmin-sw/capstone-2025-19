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

    // 점프 관련 변수
    public float jumpForce = 5f;             // 점프 힘
    private bool isGrounded = true;          // 땅에 닿아 있는지 여부

    // Raycast 방식의 지면 판정 관련 변수
    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.2f;  // 캐릭터 발 밑에서 지면까지 검사할 거리
    [SerializeField] private LayerMask groundMask;              // 'Ground' 레이어(또는 태그)에 해당하는 레이어 마스크

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

        // 땅 체크(혹은 FixedUpdate에서 해도 무방)
        CheckGround();

        //Debug.Log($"isGrounded : {isGrounded}");
        // 스페이스바로 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    /// <summary>
    /// 이동 로직
    /// </summary>
    private void Move()
    {
        // Tab 키로 상태 전환 (Inventory 열기/닫기)
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

        // 인벤토리 상태면 이동 불가
        if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 이동 방향으로 회전
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 애니메이터 상태를 갱신하는 로직
    /// </summary>
    private void UpdateAnimator()
    {
        Vector3 velocity = rb.velocity;
        float forwardSpeed = transform.InverseTransformDirection(velocity).z;

        // 필요 시 애니메이터에 속도를 전달
        // anim.SetFloat("forwardSpeed", forwardSpeed);
        // photonView.RPC("SyncAnimator", RpcTarget.Others, forwardSpeed);
    }

    /// <summary>
    /// 지면에 닿아 있는지 Raycast로 판정
    /// </summary>
    private void CheckGround()
    {
        // 플레이어 하단에서 아주 조금 위로 올린 지점에서 아래 방향으로 레이캐스트
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;

        // groundCheckDistance 거리 내에 'groundMask' 레이어가 충돌하면 땅 위에 있다고 판단
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

        // 이동 속도를 리지드바디에 직접 설정
        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    
}
