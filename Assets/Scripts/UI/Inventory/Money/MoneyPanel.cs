using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;


    public void SetMoney(int value)
    {
        moneyText.text = value.ToString();
    }
}
