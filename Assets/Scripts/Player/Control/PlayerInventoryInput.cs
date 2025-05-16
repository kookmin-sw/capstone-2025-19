using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetInventory();
        }

    }



    public void SetInventory()
    {
        if(PlayerState.Instance.GetCurrentState() == PlayerState.State.Inventory)
        {
            PlayerState.Instance.ChangeState(PlayerState.State.Idle);
        }
        else
        {
            PlayerState.Instance.ChangeState(PlayerState.State.Inventory);
        }
    }
}
