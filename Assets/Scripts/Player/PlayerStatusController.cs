using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusController : Singleton<PlayerStatusController>
{
    public PlayerStatus playerStatus;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        playerStatus = new PlayerStatus();
    }
    void Start()
    {
        //TODO player 관련 UI 최신화
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingItemBuff()
    {
        //TODO ItemBuff 효과
    }
    public void UseStamina(float value)
    {
        //TODO 실시간 Player StaminaBar에 반영
    }

    //원하는 타입의 status 변화
    public void ChangeStatus(string type, int value)
    {
        playerStatus.SetPlayerStatusValue(type, value);
        //TODO 관련 UI 최신화
    }


    public class PlayerStatus
    {
        //Hp
        //Sp
        //Weight
        //Attack

        public int currentHealth;
        public float currentStamina;

        public int playerLevel;
        public int needExpPoint;
        public int expPoint;


        Dictionary<string, float> calculateValue;


        Dictionary<string, float> itemStatus;


        Dictionary<string, int> playerStatusValue;


        Dictionary<string, float> realValue;


        #region InitDictionary
        private void InitPlayerStatusDic()
        {
            playerStatusValue = new Dictionary<string, int>();
            playerStatusValue.Add("Hp", 5);
            playerStatusValue.Add("Sp", 1);
            playerStatusValue.Add("CP", 1);
            playerStatusValue.Add("AP", 1);
        }

        private void InitItemDic()
        {
            itemStatus = new Dictionary<string, float>();
            itemStatus.Add("Hp", 0);
            itemStatus.Add("Sp", 0);
            itemStatus.Add("CP", 0);
            itemStatus.Add("AP", 0);
        }

        private void InitCalculateDic()
        {
            calculateValue = new Dictionary<string, float>();
            calculateValue.Add("Hp", 0);
            calculateValue.Add("Sp", 0);
            calculateValue.Add("CP", 0);
            calculateValue.Add("AP", 0);
        }

        private void InitRealValueDic()
        {
            realValue = new Dictionary<string, float>();
            realValue.Add("Hp", 0);
            realValue.Add("Sp", 0);
            realValue.Add("CP", 0);
            realValue.Add("AP", 0);
        }
        #endregion

        public PlayerStatus()
        {
            InitCalculateDic();
            InitItemDic();
            InitPlayerStatusDic();
            InitRealValueDic();

            playerLevel = 1;
            StatusCalculate();
            currentHealth = (int)realValue["Hp"];
            currentStamina = realValue["Sp"];
        }

        private void StatusCalculate()
        {
            realValue["Hp"] = playerStatusValue["Hp"];
            realValue["Sp"] = playerStatusValue["Sp"] * 100.0f + playerLevel * 0.0f;
            realValue["CP"] = playerStatusValue["CP"] * 10.0f + playerLevel * 1.0f;
            realValue["AP"] = 0;
            needExpPoint = 1000 * playerLevel + 2000;
        }

        public int GetCalculateValue(string type)
        {
            return (int)realValue[type] + (int)itemStatus[type];
        }
        public float GetItemStatusValue(string type)
        {
            return itemStatus[type];
        }
        public int GetPlayerStatusValue(string type)
        {
            return playerStatusValue[type];
        }

        public void SetCalculateValue(string type, float value)
        {
            calculateValue[type] = value;
        }

        public void SetItemStatusValue(string type, float value)
        {
            itemStatus[type] = value;
        }
        private void SetRealValue(string type, float value)
        {
            realValue[type] = value;
        }
        public void SetPlayerStatusValue(string type, int value)
        {
            playerStatusValue[type] = value;
            StatusCalculate();
        }


        //Item사용으로 Status 변화
        /*public void SetItem(ItemStatus itemStatus, int sign)
        {

            this.itemStatus["Hp"] += itemStatus.hpValue * sign;
            this.itemStatus["Sp"] += itemStatus.staminaPoint * sign;
            this.itemStatus["CP"] += itemStatus.overLoadValue * sign;
            this.itemStatus["AP"] += itemStatus.attackValue * sign;
        }*/


        public void ClearItemStatus()
        {
            itemStatus["Hp"] = 0;
            itemStatus["Sp"] = 0;
            itemStatus["CP"] = 0;
            itemStatus["AP"] = 0;
        }

        public void CalculateLevel(int sign)
        {
            playerLevel += sign;
            StatusCalculate();
        }
    }
}
