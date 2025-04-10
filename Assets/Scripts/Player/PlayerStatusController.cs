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
        InitPlayerStatus(); //Dic�� �⺻ ���� ���� -> VillageManger���� DB�� ����ȭ�� ������ �Ŀ� ������Ʈ
        //���߿��� DB���� ����ȭ�ؿ��� �ɷ� �ٲ�� ��.
        InitReal();

        //������ DB���� ����ȭ �� ��
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
        //������ �ƹ��͵� ����� ������ ���� DB�� ���� ���� ����� �ӽ�
        playerLevel = 1;
        needExpPoint = 10;
        //������ playerStatusValue Dic���ٰ� �����ϰ� realValue�� ���� ����ؾ� ������ ���߿�
        realValue["Exp"] = 0;
        realValue["Hp"] = 100;
        realValue["Sp"] = 100;
        realValue["Ap"] = 10;
        realValue["Wp"] = 20;
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
        //RecoverStamina(recoverSpValue);
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
