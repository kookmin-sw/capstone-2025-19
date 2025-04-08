using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldInputHandler : MonoBehaviour
{
    PlayerControls inputActions;

    [Header("Manage Inputs")]
    public Vector2 move = Vector2.zero;
    public Vector2 look = Vector2.zero;
    public bool roll;
    public bool jump;
    public bool sprint;
    public float sprintTimeOut = 0.5f;
    public bool normalAttack;
    public bool strongAttack;
    public bool lockOn;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    [Range(0.1f, 10)] public float mouseSensitivity = 1f;

    [Header("Movement Settings")]
    public bool analogMovement;

    private void OnEnable()
    {
        if(inputActions == null)
        {
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += i => move = i.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += i => look = i.ReadValue<Vector2>() * mouseSensitivity;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void UpdateInputs()
    {
        SprintInput();
        RollInput();
        JumpInput();
        LockOnInput();
        AttackInput();
    }

    private void SprintInput()
    {
        sprint = inputActions.PlayerActions.Sprint.IsPressed();
    }

    private void RollInput()
    {
        inputActions.PlayerActions.Roll.performed += i => roll = true;
    }

    private void JumpInput()
    {
        inputActions.PlayerActions.Jump.performed += i => jump = true;
    }

    private void LockOnInput()
    {
        inputActions.PlayerActions.LockOn.performed += i => lockOn = true;
    }

    private void AttackInput()
    {
        inputActions.PlayerActions.NormalAttack.performed += i => normalAttack = true;
        inputActions.PlayerActions.StrongAttack.performed += i => strongAttack = true;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

