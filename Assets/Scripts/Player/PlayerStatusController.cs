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

    //현재 플레이어 체력 (max체력은 realValue["Hp"]값
    [HideInInspector]
    public float curHp;
    [HideInInspector]
    public float curSp;

    int playerLevel;
    int needExpPoint; //다음까지 필요한 경험치

    int levelPoint; //레벨업 시 얻는 포인트

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
        PlusButton.SetActive(false);
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);

        InitPlayerStatus(); //Dic에 기본 값들 생성 -> VillageManger에서 DB에 동기화된 값으로 후에 업데이트
        //나중에는 DB에서 동기화해오는 걸로 바꿔야 함.
        InitReal();

        //스탯이 DB에서 동기화 된 후
        UpdateStatusText();
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

    private void UpdateStatusText()
    {
        //처음 게임 실행하고 계산된 RealStatus를 text에 전달
        LevelText.text = playerLevel.ToString();
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;
        ApText.text = realValue["Ap"].ToString();
    }

    //계산 시점
    //아이템이 장착되는 시점 + 물약이나 저주 + 레벨업
    //playerStatus를 기준으로 realValue를 업데이트
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
        //playerStatusValue를 기준으로 realValue를 업데이트
        StatusCalculate();

        //업데이트된 realValue의 값을 text와 동기화
        UpdateStatusText();
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

    //player 경험치가 다 차면 레벨업 가능
    public void getExp(int exp)
    {
        playerStatusValue["Exp"] += exp;
        realValue["Exp"] = playerStatusValue["Exp"];

        //경험치 얻은거 상태 업데이트
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;

        if (realValue["Exp"] >= needExpPoint)
        {
            LevelUpButton.gameObject.SetActive(true);
        }
    }

    //플레이어가 LevelUp버튼을 눌렀을 경우
    public void LevelUpStep1()
    {
        //UI 작동
        PlusButton.SetActive(true);
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(true);
        LevelUpPoint.gameObject.SetActive(true);
        PointDecomposeCompleteBnt.interactable = false; //보이기만 하고 아직 클릭은 못하도록

        //능력치 포인트 및 표시
        levelPoint = 5;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;

        //+ 옆에 표시할 완료 이후 능력치들
        //주의할 점은 realValue가 아닌 playerStatusValue를 사용해서 계산해야 한다. 
        AP_Result.text = playerStatusValue["Ap"].ToString();
    }

    //Ap+버튼을 눌렀을 경우 실행
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
        //Exp 소모
        playerStatusValue["Exp"] -= needExpPoint;

        //바뀐 능력치를 저장
        playerStatusValue["Ap"] = float.Parse(AP_Result.text);

        playerLevel++;
        StatusUpdate();
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);
    }

}
