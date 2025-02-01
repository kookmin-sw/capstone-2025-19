using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Inventory,
        Idle,

    }
    public PlayerState state;
    [SerializeField] InventoryController inventoryController;


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
    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        //if (!photonView.IsMine) return;
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
        //photonView.RPC("SyncAnimator", RpcTarget.Others, forwardSpeed);
    }

    [PunRPC]
    private void SyncAnimator(float forwardSpeed)
    {
        anim.SetFloat("forwardSpeed", forwardSpeed);
    }

    private void FixedUpdate()
    {
        //if (!photonView.IsMine) return;

        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void ChangeState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Idle:
                Debug.Log("InventoryState test");
                inventoryController.DisableInventory();
                this.state = PlayerState.Idle;
                break;
            case PlayerState.Inventory:
                Debug.Log("InventoryState test");
                inventoryController.EnableInventory();
                this.state = PlayerState.Inventory;
                break;
            default:
                break;
        }
    }

    //Function Player dectected objects
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {
            //TODO 인벤토리에 ItemIcon 빼기
            if (other.GetComponent<DropItem>() == null)
            {
                inventoryController.ExitDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                inventoryController.ExitDropItem(other.GetComponent<DropItem>());
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {
            
            //TODO 인벤토리에 ItemIcon넣기
            if (other.GetComponent<DropItem>() == null)
            {
                inventoryController.EnterDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                inventoryController.EnterDropItem(other.GetComponent<DropItem>());
            }
                
        }
    }


}
