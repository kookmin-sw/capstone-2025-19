using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static QuestManager;


public class QuestManager : Singleton<QuestManager>
{
    public GameObject QuestCanvas;

    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;
    QuestList currentQuestList;

    [SerializeField] GameObject questListObject;
    [SerializeField] GameObject questListPanel;
    [SerializeField] GameObject playerQuestListPanel;

    public GameObject acceptButton;
    public GameObject dropButton;
    public GameObject rewardButton;


    List<QuestList> dailyQuestList = new List<QuestList>();
    public List<QuestList> playerQuestList = new List<QuestList>();

    //QuestList에서 현재 player가 선택한 퀘스트
    public void setCurrentQuestList(QuestList questList)
    {
        this.currentQuestList = questList;

        //quest image 적용
        this.title.text = questList.Quest.title;
        this.content.text = generateContent(questList.Quest);
    }


    void Start()
    {
        Debug.Log("QuestManager 생성");

        //버튼 false
        acceptButton.SetActive(false);
        dropButton.SetActive(false);
        rewardButton.SetActive(false);

        //1. playerQuestList 동기화
        playerQuestList.Clear();

        //2. dailyQuestList 생성
        //ex 블루포션 3개 획득
        //dailyQuestList.Clear();
        //dailyQuestList.Add(GenerateDailyQuest());
        
        //dailyQuest 개 생성
        for(int i = 0;i<1;i++)
        {
            GameObject questList = Instantiate(questListObject);
            questList.GetComponent<QuestList>().createQuest(GenerateDailyQuest());
            questList.transform.parent = questListPanel.transform;
        }
        
    }

    public Quest GenerateDailyQuest()
    {
        Quest dailyQuest = new Quest("Item Collect",Quest.QuestType.Collection,"BluePotion",1, "get", 100);
        return dailyQuest;
    }

    public void AcceptQuest()
    {
        GameObject questList = Instantiate(questListObject);
        questList.GetComponent<QuestList>().createQuest(currentQuestList.Quest);
        questList.transform.parent = playerQuestListPanel.transform;

        playerQuestList.Add(questList.GetComponent<QuestList>());
        Debug.Log($"현재 추가된 퀘스트 title : {currentQuestList.Quest.title}");

        //받은 퀘스트 삭제
        Destroy(currentQuestList.gameObject);

        //버튼 비활성화
        acceptButton.SetActive(false);

    }

    public void dropQuest()
    {
        Destroy(currentQuestList.gameObject);
        dropButton.SetActive(false);
    }

    public void getReward()
    {
        InventoryController.Instance.money += currentQuestList.Quest.reward;
        playerQuestList.Remove(currentQuestList);
        Destroy(currentQuestList.gameObject);
    }

    public string generateContent(Quest quest)
    {
        return quest.target + quest.score + quest.result;
    }

    private void OnApplicationQuit()
    {
        //playerQuestList DB에 동기화
    }

    //아이템 획득
    public void OnItemAcquired()
    {
        foreach(QuestList questList in playerQuestList)
        {
            Debug.Log($"questList");
            Quest quest = questList.Quest;
            Debug.Log($"questList : {questList.Quest.target}");
            quest.progress = InventoryController.Instance.GetItemCount(quest.target);
            Debug.Log($"progress : {quest.progress}");
            if(quest.progress >= quest.score)
            {
                quest.isCompleted = true;
                Debug.Log("퀘스트 달성");
            }
        }
    }

    

}

public class Quest
{
    //퀘스트 타입을 만든 이유
    // : 어떤 퀘스트인지 알아야 자동 생성 가능
    public enum QuestType
    {
        Hunt,
        Collection,
        Action,
        Story,
    }

    public string id;
    public string title;
    public QuestType type;
    public string target;
    public int score;
    public string result; //획득,처치 등의 행위 결과
    public int progress = 0;
    public bool isCompleted = false;
    public int reward;

    public Quest(string title, QuestType type, string target, int score, string result, int reward)
    {
        this.id = System.Guid.NewGuid().ToString();
        this.title = title;
        this.type = type;
        this.target = target;
        this.score = score;
        this.result = result;
        this.reward = reward;
    }

}
