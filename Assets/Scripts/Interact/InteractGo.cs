using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractGo : MonoBehaviour
{
    protected bool active = false;
    public abstract void InteractObject();
    public abstract void CloseInteract();
}
