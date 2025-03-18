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

    //현재 플레이어 체력 (max체력은 realValue["Hp"]값
    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint; //다음까지 필요한
    int expPoint; //가지고 있는 경험치 포인트

    //현재 장착중인 아이템의 효과
    Dictionary<string, float> itemStatus = new Dictionary<string, float>();
    //현재 플레이어에게 적용되는 버프 혹은 저주 효과
    Dictionary<string, float> itemBuffStatus = new Dictionary<string, float>();
    //아이템 효과 반영 안된 플레이어의 스탯
    Dictionary<string, float> playerStatusValue = new Dictionary<string, float>();
    //전체를 다 계산한 스탯 효과
    Dictionary<string, float> realValue = new Dictionary<string, float>();

    protected override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        //나중에는 DB에서 동기화해오는 걸로 바꿔야 함.
        InitPlayerStatus();
    }

    public void InitPlayerStatus()
    {
        //원래는 아무것도 없어야 하지만 아직 DB를 하지 못한 관계로 임시
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
        //체력, 스테미나 바 업데이트
        UpdateHpBar();
        UpdateSpBar();
        UpdateBehaviorBool();

        //스테미나 회복
        RecoverStamina(recoverSpValue);
    }

    //스테미나 상태에 따라서 행동이 가능한지 
    void UpdateBehaviorBool()
    {
        //달리기 가능
        if(curSp < sprintStamina) canSprint = false;
        else canSprint = true;

        //구르기 가능
        if (curSp < rollingStamina) canRolling = false;
        else canRolling = true;

        //공격 가능
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
        //TODO ItemBuff 효과
    }

    //원하는 타입의 status 변화
    public void ChangeStatus(string type, int value)
    {
        //playerStatus.SetPlayerStatusValue(type, value);
        //TODO 관련 UI 최신화
    }

    //계산 시점
    //아이템이 장착되는 시점 + 물약이나 저주시
    private void StatusCalculate()
    {
        realValue["Hp"] = playerStatusValue["Hp"];
        realValue["Sp"] = playerStatusValue["Sp"] * 100.0f + playerLevel * 0.0f;
        //realValue["CP"] = playerStatusValue["CP"] * 10.0f + playerLevel * 1.0f;
        //realValue["AP"] = 0;
        needExpPoint = 10 * playerLevel;
    }

    //플레이어 데미지
    public void getDamage(int damage)
    {
        curHp -= damage;
        Debug.Log($"남은 체력 : {curHp}");
    }
    
    //public void UseStamina(float value)
    //{
    //    curSp -= value;
    //    Debug.Log($"남은 스테미나 : {curSp}");
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
            //플레이어 죽음.
            Debug.Log("플레이어 사망");
        }
    }

}
