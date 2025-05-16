using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace PlayerControl
{
	public class InputHandler : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool rolling;
		public bool lockOn;
		public bool lockOnPrevious;
		public bool lockOnNext;
		public bool attack;
		public bool inventory;
		public bool useItem;
		public bool pickup;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		[Range(0.1f, 10)] public float mouseSensitivity = 1f;

		private PlayerTrigger playerTrigger;

        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnRolling(InputValue value)
		{
			RollingInput(value.isPressed);
		}

		public void OnLockOn(InputValue value)
		{
			LockOnInput(value.isPressed);
		}

		public void OnLockOnPrevious(InputValue value)
		{
			LockOnPreviousInput(value.isPressed);
		}

		public void OnLockOnNext(InputValue value)
		{
			LockOnNextInput(value.isPressed);
		}

		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}

        public void OnInventory(InputValue value)
        {
            if (PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
			{
				InventoryController.Instance.SetStoreInventory(false);
				PlayerState.Instance.ChangeState(PlayerState.State.Idle);
				Cursor.lockState = CursorLockMode.None;

			}
			else
			{
				PlayerState.Instance.ChangeState(PlayerState.State.Inventory);
                Cursor.lockState = CursorLockMode.None;
			}
        }


		public void OnInteract(InputValue value)
		{
			Debug.Log("Input test");
			InventoryController.Instance.player.InteractObject();
		}

		public void OnUseItem(InputValue value)
		{
			UseItemInput(value.isPressed);
		}



		
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection * mouseSensitivity;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void RollingInput(bool newRollingState)
		{
			rolling = newRollingState;
		}

		public void LockOnInput(bool newLockOnState)
		{
			lockOn = newLockOnState;
		}

		public void LockOnPreviousInput(bool newLockOnPreviousState)
		{
			lockOnPrevious = newLockOnPreviousState;
		}

		public void LockOnNextInput(bool newLockOnNextState)
		{
			lockOnNext = newLockOnNextState;
		}

		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}

		public void UseItemInput(bool newUseItemState)
		{
			useItem = newUseItemState;
		}

		public void PickUpInput(bool newPickUpState)
		{
			pickup = newPickUpState;
		}

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		public void OnUseItem()
		{
			InventoryController.Instance.useItemPanel.UseItem();
		}
	}
}