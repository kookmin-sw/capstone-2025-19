using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHealth;
    public enum State
    {
        Idle,
        Invincible,
        Die
    }

    public State state;

    public void ChangeState(State state)
    {
        if (this.state == State.Die) { return; }
        switch (state)
        {
            case State.Die:
                enemyHealth.DieActive();
                break;

            default:
                break;
        }
    }

}
