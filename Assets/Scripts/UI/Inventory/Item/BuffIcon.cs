using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuffIcon : MonoBehaviour
{
    public enum BuffType
    {
        RecoveryStamina,
        RecoveryHP,
        LossHp,
        AttackPointUp,
        AttackPointDown,
        Shield,
    }
    float timeToComplete = 10f;
    [SerializeField] float timerValue;
    float fillFraction;
    [SerializeField] float setTime;

    [SerializeField] private Image childImage;

    float buffValue;


    public BuffType type;


    Sprite sprite;
    Image image;



    private void Awake()
    {
        FindComponent();
    }

    void FindComponent()
    {
        image = GetComponent<Image>();


    }
    void Start()
    {

    }

    void Update()
    {
        UpdateTimer();
    }



    void UpdateTimer()
    {
        timerValue -= Time.deltaTime;

        //지금 당장은 배리어만 있으니까
        if (timerValue <= 0)
        {
            
            //playerStatusController.DisappearBuffIcon(gameObject);
            //playerStatusController.DisappearBarrier(gameObject);
        }
        else
        {

            fillFraction = 1 - (timerValue / setTime);
            childImage.fillAmount = fillFraction;
        }
    }

    public void SetBuffIcon(Sprite sprite, float buffValue, float timeValue)
    {
        this.setTime = timeValue;
        this.timerValue = setTime;
        this.buffValue = buffValue;
        image.sprite = sprite;
    }

    public void SetType(BuffType type)
    {
        this.type = type;
    }

    public float GetValue()
    {
        return buffValue;
    }



    /*IEnumerator IncreaseAndRevertValue()
    {

    }*/
}
