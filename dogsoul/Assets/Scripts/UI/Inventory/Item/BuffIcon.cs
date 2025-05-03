using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BuffIcon : MonoBehaviour
{
    public enum BuffType
    {
        RecoveryStaminaUp,
        RecoveryHP,
        LossHp,
        AttackPointUp,
        AttackPointDown,
        Shield,
        SpeedUp,
        SpeedDown,

    }
    float timeToComplete = 10f;
    [SerializeField] float timerValue;
    float fillFraction;
    [SerializeField] float setTime;

    [SerializeField] private Image childImage;

    float buffValue;


    public BuffType type;

    private ItemEffect effect;


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
            RemoveBuffIcon();
        }
        else
        {

            fillFraction = 1 - (timerValue / setTime);
            childImage.fillAmount = fillFraction;
        }
    }

    public void SetBuffIcon(Sprite sprite, float buffValue, float timeValue, ItemEffect itemEffect)
    {
        effect = itemEffect;
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

    public void RemoveBuffIcon()
    {
        effect.RemoveEffect();
        PlayerStatusController.Instance.RemoveBuffIcon(this);
        Destroy(gameObject);
        //playerStatusController.DisappearBarrier(gameObject);
    }

    /*IEnumerator IncreaseAndRevertValue()
    {

    }*/
}
