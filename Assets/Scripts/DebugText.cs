using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : Singleton<DebugText>
{
    [SerializeField] TextMeshProUGUI text;
    

    public void Debug(string text)
    {
        this.text.text += "\n" + text;
    }
    
}
