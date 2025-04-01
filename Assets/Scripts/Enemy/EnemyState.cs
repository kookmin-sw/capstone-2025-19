using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public enum State
    {
        Idle,
        Invincible
    }

    public State state;
}
