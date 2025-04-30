using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayerControl;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController_M : PlayerControl.PlayerController
{
    protected PhotonView photonView;
    protected bool photonIsMine = true;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }
    protected override void Start()
    {
        if (!photonView.IsMine)
        {
            photonIsMine = false;
            GetComponent<InputHandler>().enabled = false;
            GetComponentInChildren<PlayerTrigger>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<LockOn>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            this.enabled = false;
        }
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (photonView.IsMine)
        {
            if (PlayerState.Instance.state == PlayerState.State.Die) return;
            Move();
            GroundedCheck();
            UseItem();
            PickUp();
            JumpAndGravity();
            Rolling();
            Attack();
        }
    }

    protected override void LateUpdate()
    {
        if (photonView.IsMine)
        {
            CameraRotation();
        }
    }
}
