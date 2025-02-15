using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : Singleton<PlayerState>
{
    public enum State
    {
        Idle,
        Inventory,

    }
    public State state;
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
            default:
                break;
        }
    }
}
