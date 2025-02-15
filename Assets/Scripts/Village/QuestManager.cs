using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static QuestManager;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;
    QuestList currentQuestList;

    [SerializeField] GameObject questListObject;
    [SerializeField] GameObject questListPanel;
    [SerializeField] GameObject playerQuestListPanel;

    public GameObject acceptButton;
    public GameObject dropButton;


    List<Quest> dailyQuestList = new List<Quest>();
    List<Quest> playerQuestList = new List<Quest>();

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
        //��ư false
        acceptButton.SetActive(false);
        dropButton.SetActive(false);

        //1. playerQuestList ����ȭ
        playerQuestList.Clear();

        //2. dailyQuestList ����
        //ex ������� 3�� ȹ��
        dailyQuestList.Clear();
        dailyQuestList.Add(GenerateDailyQuest());
        
        //QuestList�� dailyquest ����
        foreach(Quest quest in dailyQuestList)
        {
            GameObject questList = Instantiate(questListObject);
            questList.GetComponent<QuestList>().setQuest(quest);
            questList.transform.parent = questListPanel.transform;
        }
        
    }

    public Quest GenerateDailyQuest()
    {
        Quest dailyQuest = new Quest("Item Collect",Quest.QuestType.Collection,"blue_portion",3, "get");
        return dailyQuest;
    }

    public void AcceptQuest()
    {
        playerQuestList.Add(currentQuestList.Quest);
        Debug.Log($"���� �߰��� ����Ʈ title : {currentQuestList.Quest.title}");

        GameObject questList = Instantiate(questListObject);
        questList.GetComponent<QuestList>().setQuest(currentQuestList.Quest);
        questList.transform.parent = playerQuestListPanel.transform;

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

    public string generateContent(Quest quest)
    {
        return quest.target + quest.score + quest.result;
    }

    private void OnApplicationQuit()
    {
        //playerQuestList DB�� ����ȭ
    }

}

public class Quest
{
    public enum QuestType
    {
        Hunt,
        Collection,
        Action,
        Story,
    }

    public string title;
    public QuestType type;
    public string target;
    public int score;
    public string result;
    public int reword;

    public Quest(string title, QuestType type, string target, int score, string result)
    {
        this.title = title;
        this.type = type;
        this.target = target;
        this.score = score;
        this.result = result;
    }

}
