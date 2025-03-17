using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : Singleton<PlayerStatusController>
{
    [SerializeField] Slider HpBar;
    [SerializeField] Slider SpBar;

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
        realValue["Sp"] = 100;
        curHp = realValue["Hp"];
        curSp = realValue["Sp"];
    }

    // Update is called once per frame
    void Update()
    {
        //ü��, ���׹̳� �� ������Ʈ
        UpdateHpBar();
        UpdateSpBar();
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
    public void UseStamina(float value)
    {
        //TODO �ǽð� Player StaminaBar�� �ݿ�
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

    private void Die()
    {
        if(curHp <= 0)
        {
            //�÷��̾� ����.
            Debug.Log("�÷��̾� ���");
        }
    }

}
