using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Firestore;
using Firebase.Extensions;
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
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);

        Debug.Log("QuestManager ����");

        //��ư false
        acceptButton.SetActive(false);
        dropButton.SetActive(false);
        rewardButton.SetActive(false);

        //1. playerQuestList ����ȭ
        //playerQuestList.Clear();
        //LoadPlayerQuests();

        //2. dailyQuestList ����
        //ex ������� 3�� ȹ��
        //dailyQuestList.Clear();
        //dailyQuestList.Add(GenerateDailyQuest());

        //dailyQuest �� ����
        for (int i = 0;i<1;i++)
        {
            GameObject questList = Instantiate(questListObject);
            questList.GetComponent<QuestList>().createQuest(GenerateDailyQuest());
            questList.transform.parent = questListPanel.transform;
        }
        
    }

    void CheckFirebaseReady()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            CancelInvoke(nameof(CheckFirebaseReady));
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
        //SavePlayerQuestsToFirebase();
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

    
    
    //for VillageManager
    public void AddQuestToUI(Quest loadedQuest)
    {
        GameObject questListGO = Instantiate(questListObject);
        questListGO.GetComponent<QuestList>().createQuest(loadedQuest);
        questListGO.transform.parent = playerQuestListPanel.transform;
        playerQuestList.Add(questListGO.GetComponent<QuestList>());
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

//// Firebase���� ����� ����Ʈ�� �ҷ��� playerQuestList�� �߰�s
//void LoadPlayerQuests()
//{
//    var auth = FirebaseManager.Instance.Auth;
//    var db = FirebaseManager.Instance.Db;
//    var user = auth.CurrentUser;
//    CollectionReference questCollection = db.Collection("Users").Document(user.UserId).Collection("Quests");
//    questCollection.GetSnapshotAsync().ContinueWithOnMainThread(task =>
//    {
//        if (task.IsFaulted)
//        {
//            Debug.LogError("����Ʈ �ҷ����� ����: " + task.Exception);
//        }
//        else
//        {
//            QuerySnapshot snapshot = task.Result;
//            foreach (DocumentSnapshot doc in snapshot.Documents)
//            {
//                Dictionary<string, object> questData = doc.ToDictionary();

//                string questTitle = questData["title"].ToString();
//                string questTypeStr = questData["type"].ToString();
//                Quest.QuestType questType = (Quest.QuestType)System.Enum.Parse(typeof(Quest.QuestType), questTypeStr);
//                string target = questData["target"].ToString();
//                int score = int.Parse(questData["score"].ToString());
//                string result = questData["result"].ToString();
//                int reward = int.Parse(questData["reward"].ToString());
//                int progress = int.Parse(questData["progress"].ToString());
//                bool isCompleted = bool.Parse(questData["isCompleted"].ToString());

//                Quest loadedQuest = new Quest(questTitle, questType, target, score, result, reward);
//                loadedQuest.progress = progress;
//                loadedQuest.isCompleted = isCompleted;

//                // �ҷ��� ����Ʈ�� UI�� �߰�
//                GameObject questListGO = Instantiate(questListObject);
//                questListGO.GetComponent<QuestList>().createQuest(loadedQuest);
//                questListGO.transform.parent = playerQuestListPanel.transform;
//                playerQuestList.Add(questListGO.GetComponent<QuestList>());
//            }
//            Debug.Log("Firestore���� ����Ʈ �ҷ����� �Ϸ�!");
//        }
//    });
//}

//// ���� playerQuestList�� ����Ʈ ������ Firebase�� ����
//void SavePlayerQuestsToFirebase()
//{
//    var auth = FirebaseManager.Instance.Auth;
//    var db = FirebaseManager.Instance.Db;
//    var user = auth.CurrentUser;
//    CollectionReference questCollection = db.Collection("Users").Document(user.UserId).Collection("Quests");

//    // ���� ����Ʈ ���� ����
//    questCollection.GetSnapshotAsync().ContinueWithOnMainThread(deleteTask =>
//    {
//        if (deleteTask.IsFaulted)
//        {
//            Debug.LogError("���� ����Ʈ ���� ����: " + deleteTask.Exception);
//        }
//        else
//        {
//            QuerySnapshot snapshot = deleteTask.Result;
//            foreach (DocumentSnapshot doc in snapshot.Documents)
//            {
//                doc.Reference.DeleteAsync();
//            }

//            // ���� �����ִ� ����Ʈ ������ ����
//            foreach (QuestList questList in playerQuestList)
//            {
//                Quest quest = questList.Quest;
//                Dictionary<string, object> questData = new Dictionary<string, object>()
//                {
//                    { "id", quest.id },
//                    { "title", quest.title },
//                    { "type", quest.type.ToString() },
//                    { "target", quest.target },
//                    { "score", quest.score },
//                    { "result", quest.result },
//                    { "reward", quest.reward },
//                    { "progress", quest.progress },
//                    { "isCompleted", quest.isCompleted }
//                };

//                // quest.id�� ���� ID�� ���
//                DocumentReference questDoc = questCollection.Document(quest.id);
//                questDoc.SetAsync(questData).ContinueWithOnMainThread(writeTask =>
//                {
//                    if (writeTask.IsFaulted)
//                    {
//                        Debug.LogError("����Ʈ ���� ����: " + writeTask.Exception);
//                    }
//                    else
//                    {
//                        Debug.Log("����Ʈ ���� �Ϸ�: " + quest.title);
//                    }
//                });
//            }
//        }
//    });
//}