using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : Singleton<PlayerState>
{
    public enum State
    {
        Idle,
        Inventory,
        Invincible,
        Die,

    }
    private State state = State.Idle;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        state = State.Idle;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeState(State state)
    {
        if (this.state == State.Die)
        {
            return;
        }
        switch (state)
        {
            case State.Idle:
                this.state = State.Idle;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            case State.Inventory:
                this.state = State.Inventory;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            case State.Die:
                this.state = State.Die;
                InventoryController.Instance.SetInventoryCanvas();
                InventoryController.Instance.AllClearInventory();
                //PlayerStatusController.Instance.
                break;
            default:
                break;
        }
    }

    public void ChageStateHard(State state)
    {
        switch (state)
        {
            case State.Idle:
                this.state = State.Idle;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            case State.Inventory:
                this.state = State.Inventory;
                InventoryController.Instance.SetInventoryCanvas();
                break;
            case State.Die:
                this.state = State.Die;
                InventoryController.Instance.SetInventoryCanvas();
                PlayerStatusController.Instance.deadPanel.SetDeath();
                break;
            default: break;
        }
    }

    public State GetCurrentState()
    {
        return state;
    }
}
