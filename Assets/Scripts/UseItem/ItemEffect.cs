using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject 
{
    public abstract bool Effect();
    public abstract void RemoveEffect();

    public string effectContext;
}
