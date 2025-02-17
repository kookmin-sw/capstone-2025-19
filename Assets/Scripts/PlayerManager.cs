using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    Animator anim;
    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;
    public bool isInteracting;
    public bool isSprinting;
    public bool isInAir;
    public bool isGrounded;

    
    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }

    }

    // Update is called once per frame
    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandleRollingAndSprinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
    }

    void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        inputHandler.rb_Input = false;
        inputHandler.rt_Input = false;

        if (isInAir)
        {
            playerLocomotion.inAirTimer += Time.deltaTime;
        }
    }
}
