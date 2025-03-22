using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class TriggerUI : MonoBehaviour
{
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;

    public void StartEvent()
    {
        Debug.Log("2");

        enterEvent.Invoke();
    }
    public void EndEvent()
    {
        exitEvent.Invoke();
    }
    
}
