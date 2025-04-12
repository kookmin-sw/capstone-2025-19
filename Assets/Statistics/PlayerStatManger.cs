using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatManger : MonoBehaviour
{
    [SerializeField] Slider HpBar;
    [SerializeField] Slider SpBar;
    [SerializeField] Slider ExpBar;

    public float maxHp;
    public float currentHp;
    public float maxSp;
    public float currentSp;
    public float maxExp;
    public float currentExp;

    void Start()
    {
        
    }

   

    // Update is called once per frame
    void Update()
    {
        //ü��, ����, ����ġ �� ������Ʈ
        UpdateHpBar();
        UpdateSpBar();
        UpdateExpBar();


    }

    void UpdateHpBar()
    {
        HpBar.value = currentHp / maxHp;
    }
    void UpdateSpBar()
    {
        SpBar.value = currentSp / maxSp;
    }
    void UpdateExpBar()
    {
        ExpBar.value = currentExp / maxExp;
    }

    public void PlayerDamaged(float damage)
    {
        currentHp -= damage;
        if (currentHp < 0)
        {
            PlayerDied();
        }
    }

    public void PlayerDied()
    {
        Debug.Log("�÷��̾� ����");
    }

    public void ResetHp()
    {
        currentHp = maxHp;
    }
}
