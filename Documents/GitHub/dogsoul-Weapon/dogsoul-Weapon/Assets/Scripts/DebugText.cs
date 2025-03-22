using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : Singleton<DebugText>
{
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        text.raycastTarget = false;
    }

    public void Debug(string text)
    {
        this.text.text += "\n" + text;
    }
    
}
