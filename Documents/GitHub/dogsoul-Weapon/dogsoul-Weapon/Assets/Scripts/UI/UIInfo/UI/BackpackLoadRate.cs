using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BackpackLoadRate : UIInfoData
{
    public string backpackName;
    public float maxValue;
    public float currentValue;
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public override string GetContent()
    {
        if(backpackName == null)
        {
            return $"Empty hands\n {currentValue} / {maxValue}";
        }
        return $"{backpackName} \n {currentValue} / {maxValue}";
    }
    public void SetValue(float currentValue, float maxValue)
    {
        this.maxValue = maxValue;
        this.currentValue = currentValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;
        GetContent();
    }

    
}
