using System;
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
    [SerializeField] Button LevelUpButton;
    [SerializeField] Button PointDecomposeCompleteBnt;
    [SerializeField] TextMeshProUGUI LevelUpPoint;

    [Header("LevelUp")]
    [SerializeField] GameObject PlusButton;
    [SerializeField] TextMeshProUGUI AP_Result;

    [Header("Stamina")]
    [SerializeField] float recoverSpValue = 3f;
    [SerializeField] float sprintStamina = 5f;
    [SerializeField] float rollingStamina = 5f;
    [SerializeField] float attackStamina = 5f;

    [Header("player move bool")]
    public bool canSprint;
    public bool canRolling;
    public bool canAttack;

    //���� �÷��̾� ü�� (maxü���� realValue["Hp"]��
    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint; //�������� �ʿ��� ����ġ

    int levelPoint; //������ �� ��� ����Ʈ

    //�÷��̾� ���� ���� (Dic �����)
    //Hp
    //Sp
    //Ap - ���ݷ�
    //Wp - ���緮

    //���� �������� �������� ȿ�� -> ����� Hp ���� ù���ڸ� �빮��
    Dictionary<string, float> itemStatus = new Dictionary<string, float>();
    //���� �÷��̾�� ����Ǵ� ���� Ȥ�� ���� ȿ��
    Dictionary<string, float> itemBuffStatus = new Dictionary<string, float>();
    //������ ȿ�� �ݿ� �ȵ� �÷��̾��� ����
    Dictionary<string, float> playerStatusValue = new Dictionary<string, float>();
    //��ü�� �� ����� ���� ȿ��
    Dictionary<string, float> realValue = new Dictionary<string, float>();

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        PlusButton.SetActive(false);
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);

        InitPlayerStatus(); //Dic�� �⺻ ���� ���� -> VillageManger���� DB�� ����ȭ�� ������ �Ŀ� ������Ʈ
        //���߿��� DB���� ����ȭ�ؿ��� �ɷ� �ٲ�� ��.
        InitReal();

        //������ DB���� ����ȭ �� ��
        UpdateStatusText();
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
        //������ �ƹ��͵� ����� ������ ���� DB�� ���� ���� ����� �ӽ�
        playerLevel = 1;
        needExpPoint = 10;
        //������ playerStatusValue Dic���ٰ� �����ϰ� realValue�� ���� ����ؾ� ������ ���߿�
        playerStatusValue["Exp"] = 0;
        playerStatusValue["Hp"] = 1000;
        playerStatusValue["Sp"] = 10;
        playerStatusValue["Ap"] = 10;
        playerStatusValue["Wp"] = 20;

        StatusCalculate();

        curHp = realValue["Hp"];
        curSp = realValue["Sp"];
    }

    // Update is called once per frame
    void Update()
    {
        //ü��, ���׹̳� �� ������Ʈ
        UpdateHpBar();
        UpdateSpBar();
        UpdateBehaviorBool();

        //���׹̳� ȸ��
        RecoverStamina(recoverSpValue);
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

    private void UpdateStatusText()
    {
        //ó�� ���� �����ϰ� ���� RealStatus�� text�� ����
        LevelText.text = playerLevel.ToString();
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;
        ApText.text = realValue["Ap"].ToString();
    }

    //��� ����
    //�������� �����Ǵ� ���� + �����̳� ���� + ������
    //playerStatus�� �������� realValue�� ������Ʈ
    private void StatusCalculate()
    {
        realValue["Exp"] = playerStatusValue["Exp"];
        realValue["Hp"] = playerStatusValue["Hp"];
        realValue["Sp"] = playerStatusValue["Sp"] * 100.0f + playerLevel * 0.0f;
        realValue["Ap"] = playerStatusValue["Ap"];
        //realValue["Cp"] = playerStatusValue["Cp"] * 10.0f + playerLevel * 1.0f;
        needExpPoint = 10 * playerLevel;
    }

    void StatusUpdate()
    {
        //playerStatusValue�� �������� realValue�� ������Ʈ
        StatusCalculate();

        //������Ʈ�� realValue�� ���� text�� ����ȭ
        UpdateStatusText();
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

    //player ����ġ�� �� ���� ������ ����
    public void getExp(int exp)
    {
        playerStatusValue["Exp"] += exp;
        realValue["Exp"] = playerStatusValue["Exp"];

        //����ġ ������ ���� ������Ʈ
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;

        if (realValue["Exp"] >= needExpPoint)
        {
            LevelUpButton.gameObject.SetActive(true);
        }
    }

    //�÷��̾ LevelUp��ư�� ������ ���
    public void LevelUpStep1()
    {
        //UI �۵�
        PlusButton.SetActive(true);
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(true);
        LevelUpPoint.gameObject.SetActive(true);
        PointDecomposeCompleteBnt.interactable = false; //���̱⸸ �ϰ� ���� Ŭ���� ���ϵ���

        //�ɷ�ġ ����Ʈ �� ǥ��
        levelPoint = 5;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;

        //+ ���� ǥ���� �Ϸ� ���� �ɷ�ġ��
        //������ ���� realValue�� �ƴ� playerStatusValue�� ����ؼ� ����ؾ� �Ѵ�. 
        AP_Result.text = playerStatusValue["Ap"].ToString();
    }

    //Ap+��ư�� ������ ��� ����
    public void ApPlusButton()
    {
        AP_Result.text = (float.Parse(AP_Result.text) + 1).ToString();
        levelPoint--;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    private void isLevelPoint0()
    {
        if(levelPoint <= 0)
        {
            PlusButton.SetActive(false);
            PointDecomposeCompleteBnt.interactable = true;
        }
    }

    public void PressPointDecomposeCompleteBnt()
    {
        //Exp �Ҹ�
        playerStatusValue["Exp"] -= needExpPoint;

        //�ٲ� �ɷ�ġ�� ����
        playerStatusValue["Ap"] = float.Parse(AP_Result.text);

        playerLevel++;
        StatusUpdate();
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);
    }

}
