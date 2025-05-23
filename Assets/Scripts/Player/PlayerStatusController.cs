using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : Singleton<PlayerStatusController>
{
    [Header("BuffIcon")]
    [SerializeField] private Transform buffIconList;
    private List<BuffIcon> buffIconList_ = new List<BuffIcon>();
    [SerializeField] public UseItemShortCut useItemShortCut;
    [SerializeField] public Transform deadStatePanel;
    [SerializeField] public PlayerDeadPanel deadPanel;

    [Space(10)]
    [Header("Lock on")]
    [SerializeField] public LockOnImage lockOnImage;



    [Header("Slider bar")]
    [SerializeField] Slider HpBar;
    [SerializeField] Slider SpBar;

    [Header("Stat_Text")]
    [SerializeField] TextMeshProUGUI LevelText;
    [SerializeField] TextMeshProUGUI ExpText;
    [SerializeField] TextMeshProUGUI HpText;
    [SerializeField] TextMeshProUGUI SpText;
    [SerializeField] TextMeshProUGUI ApText;
    [SerializeField] TextMeshProUGUI WpText;
    [SerializeField] Button LevelUpButton;
    [SerializeField] Button PointDecomposeCompleteBnt;
    [SerializeField] TextMeshProUGUI LevelUpPoint;

    [Header("LevelUp")]
    [SerializeField] GameObject HpPlusButton;
    [SerializeField] GameObject HpMinusButton;
    [SerializeField] TextMeshProUGUI HP_Result;
    [SerializeField] GameObject SpPlusButton;
    [SerializeField] GameObject SpMinusButton;
    [SerializeField] TextMeshProUGUI SP_Result;
    [SerializeField] GameObject ApPlusButton;
    [SerializeField] GameObject ApMinusButton;
    [SerializeField] TextMeshProUGUI AP_Result;
    [SerializeField] GameObject WpPlusButton;
    [SerializeField] GameObject WpMinusButton;
    [SerializeField] TextMeshProUGUI WP_Result;

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

    //현재 플레이어 체력 (max체력은 realValue["Hp"]값
    public float curHp;
    public float curSp;

    public string playerNickname;
    int playerLevel = 1;
    int needExpPoint; //다음까지 필요한 경험치

    int levelPoint; //레벨업 시 얻는 포인트
    int usedLevelPoint = 0;

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
        Plus_Minus_Button_SetActive_False();
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);

        SetDictionaryKey(); //SetDictionary Before SynchronizeDB
        //TestLoadPlayerStatus();
    }

    public float GetRealValue(string key)
    {
        return realValue[key];
    }

    public void SetDictionaryKey()
    {
        playerStatusValue["Exp"] = 0;
        playerStatusValue["Hp"] = 10;
        playerStatusValue["Sp"] = 10;
        playerStatusValue["Ap"] = 10;
        playerStatusValue["Wp"] = 10;

        realValue["Exp"] = 10;
        realValue["Hp"] = 10;
        realValue["Sp"] = 10;
        realValue["Ap"] = 10;
        realValue["Wp"] = 10;

        itemBuffStatus["Hp"] = 0;
        itemBuffStatus["Sp"] = 0;
        itemBuffStatus["Ap"] = 0;
        itemBuffStatus["Wp"] = 0;
    }


    public VillageManager.Status GetPlayerStatus()
    {
        VillageManager.Status status = new VillageManager.Status();
        status.level = playerLevel;
        status.exp = (int)playerStatusValue["Exp"];
        status.hp = playerStatusValue["Hp"];
        status.sp = playerStatusValue["Sp"];
        status.ap = playerStatusValue["Ap"];
        status.wp = playerStatusValue["Wp"];

        return status;
    }

    void TestLoadPlayerStatus()
    {
        playerStatusValue["Hp"] = 100;
        playerStatusValue["Sp"] = 100;

        //Calculate stat include weapon and item effect
        InitReal();

        //show stat to statusCanvas
        UpdateStatusText();
    }

    public void LoadPlayerStatus(VillageManager.Status status)
    {
        playerLevel = status.level;
        playerStatusValue["Exp"] = status.exp;
        playerStatusValue["Hp"] = status.hp;
        playerStatusValue["Sp"] = status.sp;
        playerStatusValue["Ap"] = status.ap;
        playerStatusValue["Wp"] = status.wp;

        //Calculate stat include weapon and item effect
        InitReal();

        //show stat to statusCanvas
        UpdateStatusText();

        if ((int)playerStatusValue["Exp"] >= needExpPoint)
        {
            LevelUpButton.gameObject.SetActive(true);
        }
    }

    public void InitReal()
    {
        needExpPoint = playerLevel * (playerLevel + 2);
        StatusCalculate();

        curHp = realValue["Hp"];
        curSp = realValue["Sp"];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHpBar();
        UpdateSpBar();
        UpdateBehaviorBool();

        //스테미나 회복
        if (recoverStamina && curSp < realValue["Sp"])
        {
            curSp += Time.deltaTime * recoverSpValue;
        }

        if (chargeStaminaDelta < timeToChargeStamina) chargeStaminaDelta += Time.deltaTime;
        else recoverStamina = true;
    }



    //스테미나 상태에 따라서 행동이 가능한지 
    void UpdateBehaviorBool()
    {
        //달리기 가능
        if (curSp < sprintStamina) canSprint = false;
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

        float HpBarValue = curHp / realValue["Hp"];
        if (HpBarValue != float.NaN) HpBar.value = HpBarValue;

        //Debug.Log($"현재 Hp : {HpBar.value}, curHp : {curHp}, realValue[Hp] : {realValue["Hp"]}");
    }
    void UpdateSpBar()
    {
        SpBar.value = curSp / realValue["Sp"];
    }

    private void UpdateStatusText()
    {
        LevelText.text = playerLevel.ToString();
        ExpText.text = realValue["Exp"].ToString() + " / " + needExpPoint;
        HpText.text = realValue["Hp"].ToString();
        SpText.text = realValue["Sp"].ToString();
        ApText.text = realValue["Ap"].ToString();
        WpText.text = realValue["Wp"].ToString();
    }


    private void StatusCalculate()
    {
        realValue["Exp"] = playerStatusValue["Exp"];
        realValue["Hp"] = playerStatusValue["Hp"] + itemBuffStatus["Hp"];
        realValue["Sp"] = playerStatusValue["Sp"] + itemBuffStatus["Sp"];
        realValue["Ap"] = playerStatusValue["Ap"] + itemBuffStatus["Ap"];
        realValue["Wp"] = playerStatusValue["Wp"] + itemBuffStatus["Wp"];
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

    public void UseStamina(float usage)
    {
        curSp -= usage;
        chargeStaminaDelta = 0;
        recoverStamina = false;
        if (curSp <= 0) curSp = 0;
    }

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

    //private void RecoverStamina(float recoverSpValue)
    //{
    //    if (realValue["Sp"] > curSp) 
    //    {
    //        curSp += recoverSpValue* Time.deltaTime;
    //    }
    //}

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
        Plus_Minus_Button_SetActive_True();
        LevelUpButton.gameObject.SetActive(false);
        PointDecomposeCompleteBnt.gameObject.SetActive(true);
        LevelUpPoint.gameObject.SetActive(true);
        PointDecomposeCompleteBnt.interactable = false; //보이기만 하고 아직 클릭은 못하도록

        //능력치 포인트 및 표시
        levelPoint = 5;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;

        //+ 옆에 표시할 완료 이후 능력치들
        //주의할 점은 realValue가 아닌 playerStatusValue를 사용해서 계산해야 한다. 
        HP_Result.text = playerStatusValue["Hp"].ToString();
        SP_Result.text = playerStatusValue["Sp"].ToString();
        AP_Result.text = playerStatusValue["Ap"].ToString();
        WP_Result.text = playerStatusValue["Wp"].ToString();
    }

    public void Plus_Minus_Button_SetActive_True()
    {
        HpPlusButton.SetActive(true);
        HpMinusButton.SetActive(true);
        SpPlusButton.SetActive(true);
        SpMinusButton.SetActive(true);
        ApPlusButton.SetActive(true);
        ApMinusButton.SetActive(true);
        WpPlusButton.SetActive(true);
        WpMinusButton.SetActive(true);
    }

    public void Plus_Minus_Button_SetActive_False()
    {
        HpPlusButton.SetActive(false);
        HpMinusButton.SetActive(false);
        SpPlusButton.SetActive(false);
        SpMinusButton.SetActive(false);
        ApPlusButton.SetActive(false);
        ApMinusButton.SetActive(false);
        WpPlusButton.SetActive(false);
        WpMinusButton.SetActive(false);
    }

    #region Stat_plus_minus

    //+버튼을 눌렀을 경우 실행
    public void HpPlus()
    {
        if (levelPoint <= 0) return;
        HP_Result.text = (float.Parse(HP_Result.text) + 1).ToString();
        levelPoint--;
        usedLevelPoint++;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void HpMinus()
    {
        if (usedLevelPoint <= 0) return;
        HP_Result.text = (float.Parse(HP_Result.text) - 1).ToString();
        levelPoint++;
        usedLevelPoint--;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void SpPlus()
    {
        if (levelPoint <= 0) return;
        SP_Result.text = (float.Parse(SP_Result.text) + 1).ToString();
        levelPoint--;
        usedLevelPoint++;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void SpMinus()
    {
        if (usedLevelPoint <= 0) return;
        SP_Result.text = (float.Parse(SP_Result.text) - 1).ToString();
        levelPoint++;
        usedLevelPoint--;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void ApPlus()
    {
        if (levelPoint <= 0) return;
        AP_Result.text = (float.Parse(AP_Result.text) + 1).ToString();
        levelPoint--;
        usedLevelPoint++;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void ApMinus()
    {
        if (usedLevelPoint <= 0) return;
        AP_Result.text = (float.Parse(AP_Result.text) - 1).ToString();
        levelPoint++;
        usedLevelPoint--;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void WpPlus()
    {
        if (levelPoint <= 0) return;
        WP_Result.text = (float.Parse(WP_Result.text) + 1).ToString();
        levelPoint--;
        usedLevelPoint++;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    public void WpMinus()
    {
        if (usedLevelPoint <= 0) return;
        WP_Result.text = (float.Parse(WP_Result.text) - 1).ToString();
        levelPoint++;
        usedLevelPoint--;
        LevelUpPoint.text = "LevelPoint : " + levelPoint;
        isLevelPoint0();
    }

    #endregion
    private void isLevelPoint0()
    {
        if (levelPoint <= 0)
        {
            PointDecomposeCompleteBnt.interactable = true;
        }
        else
        {
            PointDecomposeCompleteBnt.interactable = false;
        }
    }

    public void PressPointDecomposeCompleteBnt()
    {
        //Exp 소모
        playerStatusValue["Exp"] -= needExpPoint;

        //바뀐 능력치를 저장
        playerStatusValue["Hp"] = float.Parse(HP_Result.text);
        playerStatusValue["Sp"] = float.Parse(SP_Result.text);
        playerStatusValue["Ap"] = float.Parse(AP_Result.text);
        playerStatusValue["Wp"] = float.Parse(WP_Result.text);

        playerLevel++;
        needExpPoint = playerLevel * (playerLevel + 2);
        StatusUpdate();
        PointDecomposeCompleteBnt.gameObject.SetActive(false);
        LevelUpPoint.gameObject.SetActive(false);

        HP_Result.text = "";
        SP_Result.text = "";
        AP_Result.text = "";
        WP_Result.text = "";
        Plus_Minus_Button_SetActive_False();
    }

    private GameObject CreateBuffIcon(BuffIcon.BuffType type, float value, float timeValue, ItemEffect itemEffect)
    {
        GameObject buffIconGo = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Inventory/BuffIcon"), buffIconList);
        BuffIcon buffIcon = buffIconGo.GetComponent<BuffIcon>();
        buffIcon.SetType(type);
        Sprite sprite = Resources.Load<Sprite>($"Sprites/PlayerBuffIcon/{type.ToString()}Icon");
        buffIcon.SetBuffIcon(sprite, value, timeValue, itemEffect);
        buffIconList_.Add(buffIcon);
        return buffIconGo;
    }

    public void RemoveBuffIcon(BuffIcon buffIcon)
    {
        buffIconList_.Remove(buffIcon);
    }

    private bool CheckBuffType(BuffIcon.BuffType type, out BuffIcon buffIcon_)
    {
        foreach(BuffIcon buffIcon in buffIconList_)
        {
            Debug.Log(buffIcon.type);
            Debug.Log($"type is {type}");
            if(buffIcon.type == type)
            {
                buffIcon_ = buffIcon;
                return true;
            }
        }
        buffIcon_ = null;
        return false;
    }

    #region buffIconFunction
    public void RecoveryStaminaBuff(float recoveryValue, float timeValue, ItemEffect itemEffect)
    {
        if(CheckBuffType(BuffIcon.BuffType.RecoveryStaminaUp, out BuffIcon buffIcon_)) { buffIcon_.RemoveBuffIcon(); }
        //if (recoverySp != null) { DisappearBuffIcon(recoverySp.gameObject); }
        GameObject buffIcon = CreateBuffIcon(BuffIcon.BuffType.RecoveryStaminaUp, recoveryValue, timeValue, itemEffect);
        Debug.Log(recoverSpValue);
        recoverSpValue += recoveryValue;
        Debug.Log(recoverSpValue);
    }
    
    public void ResetStaminaBuff(float recoveryValue) { 
        recoverSpValue -= recoveryValue;
    }

    public void AttackPointUpBuff(float  attackPointUpValue, float timeValue, ItemEffect itemEffect)
    {
        if (CheckBuffType(BuffIcon.BuffType.AttackPointUp, out BuffIcon buffIcon_)) { buffIcon_.RemoveBuffIcon(); }
        //if (recoverySp != null) { DisappearBuffIcon(recoverySp.gameObject); }
        GameObject buffIcon = CreateBuffIcon(BuffIcon.BuffType.AttackPointUp, attackPointUpValue, timeValue, itemEffect);
        itemBuffStatus["Ap"] += attackPointUpValue;
        StatusCalculate();
    }

    public void ResetAttackPointUpBuff(float attackPointUpValue)
    {
        itemBuffStatus["Ap"] -= attackPointUpValue;
        StatusCalculate();
    }

    public void SpeedUpBuff(float speedUpValue, float timeValue, ItemEffect itemEffect) 
    {
        if (CheckBuffType(BuffIcon.BuffType.SpeedUp, out BuffIcon buffIcon_)) { buffIcon_.RemoveBuffIcon(); }
        //if (recoverySp != null) { DisappearBuffIcon(recoverySp.gameObject); }
        GameObject buffIcon = CreateBuffIcon(BuffIcon.BuffType.SpeedUp, speedUpValue, timeValue, itemEffect);
        InventoryController.Instance.playerController.MoveSpeed += speedUpValue;
        InventoryController.Instance.playerController.SprintSpeed += speedUpValue;

    }

    public void ResetSpeed(float speedUpValue)
    {
        InventoryController.Instance.playerController.MoveSpeed -= speedUpValue;
        InventoryController.Instance.playerController.SprintSpeed -= speedUpValue;
    }


    #endregion


}
