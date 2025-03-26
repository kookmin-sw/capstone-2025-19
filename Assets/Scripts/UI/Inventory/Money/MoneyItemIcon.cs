using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MoneyItemIcon : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyValueText;

    public int moneyValue = 1;
    public MoneyDropItem dropItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMoney(int money)
    {
        moneyValue = money;
        moneyValueText.text = moneyValue.ToString();
    }

    public void RemoveItemIcon()
    {
        Destroy(gameObject);
    }
}
