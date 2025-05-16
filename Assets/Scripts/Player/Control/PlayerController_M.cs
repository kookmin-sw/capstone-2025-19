using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using PlayerControl;
using RPGCharacterAnims.Lookups;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController_M : PlayerControl.PlayerController
{
    protected PhotonView photonView;
    protected bool photonIsMine = true;
    protected WeaponStats weaponStats;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>(); if (!photonView.IsMine)
        {
            var pi = GetComponent<PlayerInput>();
            if (pi)
            {
                // 1) 모든 장치 언페어
                pi.user.UnpairDevices();
                // 2) 입력 비활성
                pi.DeactivateInput();
                // 3) 컴포넌트 자체 off
                pi.enabled = false;
            }
        }
    }
    protected override void Start()
    {
        Debug.Log("start test");
        base.Start();
        if (!photonView.IsMine)
        {
            photonIsMine = false;
            GetComponent<InputHandler>().enabled = false;
            GetComponentInChildren<PlayerTrigger>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<LockOn>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            //this.enabled = false;
            Debug.Log("false test");
            Debug.Log($"player name {gameObject.name}");
        }
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (photonView.IsMine)
        {
            if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Die) return;
            Move();
            GroundedCheck();
            UseItem();
            PickUp();
            JumpAndGravity();
            Rolling();
            Attack();
        }
    }

    protected override void Attack()
    {
        if (_input.attack)
        {
            WeaponStats weapon = InventoryController.Instance.weaponPanel.GetWeapon();
            print(weapon);
            _input.attack = false;
            if (!Grounded) return;
            if (PlayerStatusController.Instance.curSp <= 0) return;
            if (weapon == null) return;
            if (!animationHandler.GetBool(AnimationHandler.AnimParam.Blocking)
                || animationHandler.GetBool(AnimationHandler.AnimParam.CanDoCombo))
            {
                if (!weapon.isRanged) animationHandler.SetTrigger(AnimationHandler.AnimParam.Attack);
                else animationHandler.SetTrigger(AnimationHandler.AnimParam.RangedAttack);
                photonView.RPC(nameof(RpcAnimator), RpcTarget.OthersBuffered);

                animationHandler.RootMotion(true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Interacting, true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Blocking, true);
                animationHandler.SetBool(AnimationHandler.AnimParam.Attacking, true);
                PlayerStatusController.Instance.UseStamina(weapon.staminaUsage);
            }
        }
    }

    protected override void LateUpdate()
    {
        if (photonView.IsMine)
        {
            CameraRotation();
        }
    }

    [PunRPC]
    private void RpcAnimator()
    {
        if (!weaponStats.isRanged) animationHandler.SetTrigger(AnimationHandler.AnimParam.Attack);
        else animationHandler.SetTrigger(AnimationHandler.AnimParam.RangedAttack);
    }
}
