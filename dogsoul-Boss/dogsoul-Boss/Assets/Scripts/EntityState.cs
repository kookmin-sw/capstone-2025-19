using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityState : MonoBehaviour
{
    public enum State
    {
        Hit,
        Invincible,
        Stun,
        Die
    }
}
