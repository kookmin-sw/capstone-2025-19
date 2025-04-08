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

    //현재 플레이어 체력 (max체력은 realValue["Hp"]값
    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint; //다음까지 필요한 경험치

    //플레이어 스탯 종류 (Dic 저장된)
    //Hp
    //Sp
    //Ap - 공격력
    //Wp - 적재량

    //현재 장착중인 아이템의 효과 -> 저장시 Hp 같이 첫글자만 대문자
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
        InitPlayerStatus(); //Dic에 기본 값들 생성 -> VillageManger에서 DB에 동기화된 값으로 후에 업데이트
        //나중에는 DB에서 동기화해오는 걸로 바꿔야 함.
        InitReal();

        //스탯이 DB에서 동기화 된 후
        ShowFirstStatus();
    }

    private void InitPlayerStatus()
    {
        //key만 생성
        playerStatusValue["Exp"] = 0;
        playerStatusValue["Hp"] = 0;
        playerStatusValue["Sp"] = 0;
        playerStatusValue["Ap"] = 0;
        playerStatusValue["Wp"] = 0;
    }

    public void InitReal()
    {
        //원래는 아무것도 없어야 하지만 아직 DB를 하지 못한 관계로 임시
        playerLevel = 1;
        needExpPoint = 10;
        //원래는 playerStatusValue Dic에다가 저장하고 realValue에 최종 계산해야 하지만 나중에
        realValue["Exp"] = 0;
        realValue["Hp"] = 1000;
        realValue["Sp"] = 10;
        realValue["Ap"] = 10;
        realValue["Wp"] = 20;
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

    private void ShowFirstStatus()
    {
        //처음 게임 실행하고 계산된 RealStatus를 text에 전달
        LevelText.text = playerLevel.ToString();
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;
        ApText.text = realValue["Ap"].ToString();
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

    void StatusUpdate(string stat)
    {
        //업데이트된 realValue의 값을 text와 동기화
    }

    //플레이어 데미지
    public void getDamage(int damage)
    {
        curHp -= damage;
        //Debug.Log($"남은 체력 : {curHp}");
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
