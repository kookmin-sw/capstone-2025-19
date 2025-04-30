using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuffIcon : MonoBehaviour
{
    float timeToComplete = 10f;
    [SerializeField] float timerValue;
    float fillFraction;
    [SerializeField] float setTime;

    //public StatusBuff statusBuff;

    Dictionary<string, float> statusBuffDic;


    string type;


    Sprite sprite;
    Image image;

    Image childImage;

    PlayerStatusController playerStatusController;

    private void Awake()
    {
        FindComponent();
    }

    void FindComponent()
    {
        image = GetComponent<Image>();


        Transform child = transform.GetChild(0);
        childImage = child.GetComponent<Image>();

        //statusBuff = new StatusBuff();
        statusBuffDic = new Dictionary<string, float>();
        playerStatusController = GameObject.Find("PlayerStatusPanel").GetComponent<PlayerStatusController>();
        InitDictionary();
    }
    void Start()
    {

    }

    void Update()
    {
        UpdateTimer();
    }


    void InitDictionary()
    {
        statusBuffDic.Add("plusMaxHp", 0);
        statusBuffDic.Add("plusMaxSp", 0);
        statusBuffDic.Add("plusAttackPoint", 0);
        statusBuffDic.Add("plusWeight", 0);
        statusBuffDic.Add("recoveryHP", 0);
        statusBuffDic.Add("recoverySP", 0);
        statusBuffDic.Add("plusArmor", 0);
        statusBuffDic.Add("changed", 0);
        statusBuffDic.Add("durationTime", 0);
    }
    void UpdateTimer()
    {
        timerValue -= Time.deltaTime;

        //지금 당장은 배리어만 있으니까
        if (timerValue <= 0)
        {
            if (playerStatusController == null) { FindComponent(); }
            //playerStatusController.DisappearBuffIcon(gameObject);
            //playerStatusController.DisappearBarrier(gameObject);
        }
        else
        {

            fillFraction = 1 - (timerValue / setTime);
            childImage.fillAmount = fillFraction;
        }
    }

    public void SetBuffIcon(Sprite sprite, float timer)
    {
        this.setTime = timer;
        this.timerValue = setTime;
        image.sprite = sprite;
    }

    public void SetType(string type, float value)
    {
        statusBuffDic[type] = value;
    }

    public float GetValue(string type)
    {
        return statusBuffDic[type];
    }



    /*IEnumerator IncreaseAndRevertValue()
    {

    }*/
}
