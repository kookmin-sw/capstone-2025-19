using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class MoneyDropItem : MonoBehaviour
{
    public GameObject MoneyItemIcon;
    public int moneyValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void OnEnable()
    {
        Init();
    }


    protected virtual void Init()
    {
        
    }

    public virtual void SetValue(int moneyValue)
    {
        UpdateValue(moneyValue);
    }
    


    protected void UpdateValue(int moneyValue)
    {
        this.moneyValue = moneyValue;
    }
    public virtual void RemoveDropItem()
    {
        MoneyItemIcon = null;
        //InventoryController.Instance.dropItemList.Remove(this);
        gameObject.SetActive(false);

    }
    

}
