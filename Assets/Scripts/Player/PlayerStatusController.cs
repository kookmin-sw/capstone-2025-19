using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : Singleton<PlayerStatusController>
{
    [SerializeField] Slider HpBar;
    [SerializeField] Slider SpBar;

    [SerializeField] float recoverSpValue = 3f;
    [SerializeField] float sprintStamina = 5f;
    [SerializeField] float rollingStamina = 5f;
    [SerializeField] float attackStamina = 5f;

    public bool canSprint;
    public bool canRolling;
    public bool canAttack;

    //���� �÷��̾� ü�� (maxü���� realValue["Hp"]��
    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint; //�������� �ʿ���
    int expPoint; //������ �ִ� ����ġ ����Ʈ

    //���� �������� �������� ȿ��
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
        //���߿��� DB���� ����ȭ�ؿ��� �ɷ� �ٲ�� ��.
        InitPlayerStatus();
    }

    public void InitPlayerStatus()
    {
        //������ �ƹ��͵� ����� ������ ���� DB�� ���� ���� ����� �ӽ�
        playerLevel = 1;
        needExpPoint = 10;
        expPoint = 0;
        realValue["Hp"] = 1000;
        realValue["Sp"] = 10;
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

    public void SettingItemBuff()
    {
        //TODO ItemBuff ȿ��
    }

    //���ϴ� Ÿ���� status ��ȭ
    public void ChangeStatus(string type, int value)
    {
        //playerStatus.SetPlayerStatusValue(type, value);
        //TODO ���� UI �ֽ�ȭ
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

    //�÷��̾� ������
    public void getDamage(int damage)
    {
        curHp -= damage;
        Debug.Log($"���� ü�� : {curHp}");
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
