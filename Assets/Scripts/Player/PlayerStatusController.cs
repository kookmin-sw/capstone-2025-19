using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : Singleton<PlayerStatusController>
{
    [Header("Slider bar")]
    [SerializeField] Slider HpBar;
    [SerializeField] Slider SpBar;

    [Header("Stat_Text")]
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] TextMeshProUGUI ExpText;
    [SerializeField] TextMeshProUGUI ApText;


    [Header("Stamina")]
    [SerializeField] float recoverSpValue = 20f;
    [SerializeField] float sprintStamina = 5f;
    [SerializeField] float rollingStamina = 5f;
    [SerializeField] float attackStamina = 5f;
    [SerializeField] float timeToChargeStamina = 2f;

    private bool recoverStamina = false;
    private float chargeStaminaDelta = 0;

    [Header("player move bool")]
    public bool canSprint;
    public bool canRolling;
    public bool canAttack;

    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint;

    Dictionary<string, float> itemStatus = new Dictionary<string, float>();
    Dictionary<string, float> itemBuffStatus = new Dictionary<string, float>();
    Dictionary<string, float> playerStatusValue = new Dictionary<string, float>();
    public Dictionary<string, float> realValue = new Dictionary<string, float>();

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        InitPlayerStatus();
        InitReal();
        ShowFirstStatus();
    }

    private void InitPlayerStatus()
    {
        //key�� ����
        playerStatusValue["Exp"] = 0;
        playerStatusValue["Hp"] = 0;
        playerStatusValue["Sp"] = 0;
        playerStatusValue["Ap"] = 0;
        playerStatusValue["Wp"] = 0;
    }

    public void InitReal()
    {
        playerLevel = 1;
        needExpPoint = 10;
        realValue["Exp"] = 0;
        realValue["Hp"] = 100;
        realValue["Sp"] = 100;
        realValue["Ap"] = 10;
        realValue["Wp"] = 20;
        curHp = realValue["Hp"];
        curSp = realValue["Sp"];
    }

    void Update()
    {
        UpdateHpBar();
        UpdateSpBar();
        UpdateBehaviorBool();

        //���׹̳� ȸ��
        //RecoverStamina(recoverSpValue);

        if (recoverStamina && curSp < realValue["Sp"])
        {
            curSp += Time.deltaTime * recoverSpValue;
        }

        if (chargeStaminaDelta < timeToChargeStamina) chargeStaminaDelta += Time.deltaTime;
        else recoverStamina = true;
    }

    

    //���׹̳� ���¿� ���� �ൿ�� �������� 
    void UpdateBehaviorBool()
    {
        //�޸��� ����
        if(curSp < sprintStamina) canSprint = false;
        else canSprint = true;

        //������ ����
        if (curSp < rollingStamina) canRolling = false;
        else canRolling = true;

        //���� ����
        if (curSp < attackStamina) canAttack = false;
        else canAttack = true;
    }

    void UpdateHpBar()
    {
        HpBar.value = curHp / realValue["Hp"];
    }
    void UpdateSpBar()
    {
        SpBar.value = curSp / realValue["Sp"];
    }

    private void ShowFirstStatus()
    {
        //ó�� ���� �����ϰ� ���� RealStatus�� text�� ����
        LevelText.text = playerLevel.ToString();
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;
        ApText.text = realValue["Ap"].ToString();
    }

    //��� ����
    //�������� �����Ǵ� ���� + �����̳� ���ֽ�
    private void StatusCalculate()
    {
        realValue["Hp"] = playerStatusValue["Hp"];
        realValue["Sp"] = playerStatusValue["Sp"] * 100.0f + playerLevel * 0.0f;
        //realValue["CP"] = playerStatusValue["CP"] * 10.0f + playerLevel * 1.0f;
        //realValue["AP"] = 0;
        needExpPoint = 10 * playerLevel;
    }

    void StatusUpdate(string stat)
    {
        //������Ʈ�� realValue�� ���� text�� ����ȭ
    }

    //�÷��̾� ������
    public void getDamage(int damage)
    {
        curHp -= damage;
        //Debug.Log($"���� ü�� : {curHp}");
    }
    
    //public void UseStamina(float value)
    //{
    //    curSp -= value;
    //    Debug.Log($"���� ���׹̳� : {curSp}");
    //}

    public void rolling()
    {
        curSp -= rollingStamina;
    }

    public void sprint()
    {
        curSp -= sprintStamina * Time.deltaTime;
    }

    public void attack()
    {
        curSp -= attackStamina;
    }

    public void UseStamina(float usage)
    {
        curSp -= usage;
        chargeStaminaDelta = 0;
        recoverStamina = false;
        if (curSp <= 0) curSp = 0;
    }

    private void RecoverStamina(float recoverSpValue)
    {
        if (realValue["Sp"] > curSp) 
        {
            curSp += recoverSpValue* Time.deltaTime;
        }
    }


    private void Die()
    {
        if(curHp <= 0)
        {
            //�÷��̾� ����.
            Debug.Log("�÷��̾� ���");
        }
    }

}
