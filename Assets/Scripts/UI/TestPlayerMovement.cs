using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    [SerializeField] public Transform dropItemPosition;
    [SerializeField] Collider triggerCollider;
    public enum PlayerState
    {
        Inventory,
        Idle,

    }
    public PlayerState state;

    PhotonView photonView;

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
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
        ChangeState(PlayerState.Idle);
        if (!photonView.IsMine)
        {
            triggerCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        UpdateAnimator();
        Move();
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (state == PlayerState.Inventory) { ChangeState(PlayerState.Idle); } else if (state == PlayerState.Idle) { ChangeState(PlayerState.Inventory); }
        }
        if (state == PlayerState.Inventory) { return; }
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
        //print(forwardSpeed.ToString("F2"));
        anim.SetFloat("forwardSpeed", forwardSpeed);
        photonView.RPC("SyncAnimator", RpcTarget.Others, forwardSpeed);
    }
    private void ChangeState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                this.state = PlayerState.Idle;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            case PlayerState.Inventory:
                this.state = PlayerState.Inventory;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {

            //TODO 인벤토리에 ItemIcon넣기
            if (other.GetComponent<DropItem>() == null)
            {
                InventoryController.Instance.EnterDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                InventoryController.Instance.EnterDropItem(other.GetComponent<DropItem>());
            }

        }else if (other.CompareTag("Test"))
        {
            other.GetComponent<TestClass>().SetUpdate(InventoryController.Instance.testID);
            //other.GetComponent<TestClass>().testID = InventoryController.Instance.testID;
            Debug.Log($"testPhoton {other.GetComponent<TestClass>().testID.ID}");
        }
    }
    //Function Player dectected objects
    private void OnTriggerExit(Collider other)
    {
        DebugText.Instance.Debug("trigger exit test");
        if (other.CompareTag("DropItem"))
        {
            //TODO 인벤토리에 ItemIcon 빼기
            if (other.GetComponent<DropItem>() == null)
            {
                InventoryController.Instance.ExitDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                InventoryController.Instance.ExitDropItem(other.GetComponent<DropItem>());
            }

        }
    }
    [PunRPC]
    private void SyncAnimator(float forwardSpeed)
    {
        anim.SetFloat("forwardSpeed", forwardSpeed);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    

    

    


}
