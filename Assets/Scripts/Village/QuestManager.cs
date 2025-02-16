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

    //QuestList���� ���� player�� ������ ����Ʈ
    public void setCurrentQuestList(QuestList questList)
    {
        this.currentQuestList = questList;

        //quest image ����
        this.title.text = questList.Quest.title;
        this.content.text = generateContent(questList.Quest);
    }


    void Start()
    {
        Debug.Log("QuestManager ����");

        //��ư false
        acceptButton.SetActive(false);
        dropButton.SetActive(false);
        rewardButton.SetActive(false);

        //1. playerQuestList ����ȭ
        playerQuestList.Clear();

        //2. dailyQuestList ����
        //ex ������� 3�� ȹ��
        //dailyQuestList.Clear();
        //dailyQuestList.Add(GenerateDailyQuest());
        
        //dailyQuest �� ����
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
        Debug.Log($"���� �߰��� ����Ʈ title : {currentQuestList.Quest.title}");

        //���� ����Ʈ ����
        Destroy(currentQuestList.gameObject);

        //��ư ��Ȱ��ȭ
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
        //playerQuestList DB�� ����ȭ
    }

    //������ ȹ��
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
                Debug.Log("����Ʈ �޼�");
            }
        }
    }

    

}

public class Quest
{
    //����Ʈ Ÿ���� ���� ����
    // : � ����Ʈ���� �˾ƾ� �ڵ� ���� ����
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
    public string result; //ȹ��,óġ ���� ���� ���
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
