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
        InvokeRepeating(nameof(CheckFirebaseReady), 0.5f, 0.5f);

        Debug.Log("QuestManager 생성");

        //버튼 false
        acceptButton.SetActive(false);
        dropButton.SetActive(false);
        rewardButton.SetActive(false);

        //1. playerQuestList 동기화
        //playerQuestList.Clear();
        //LoadPlayerQuests();

        //2. dailyQuestList 생성
        //ex 블루포션 3개 획득
        //dailyQuestList.Clear();
        //dailyQuestList.Add(GenerateDailyQuest());

        //dailyQuest 개 생성
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
        //SavePlayerQuestsToFirebase();
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

//// Firebase에서 저장된 퀘스트를 불러와 playerQuestList에 추가s
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
//            Debug.LogError("퀘스트 불러오기 실패: " + task.Exception);
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

//                // 불러온 퀘스트를 UI에 추가
//                GameObject questListGO = Instantiate(questListObject);
//                questListGO.GetComponent<QuestList>().createQuest(loadedQuest);
//                questListGO.transform.parent = playerQuestListPanel.transform;
//                playerQuestList.Add(questListGO.GetComponent<QuestList>());
//            }
//            Debug.Log("Firestore에서 퀘스트 불러오기 완료!");
//        }
//    });
//}

//// 현재 playerQuestList의 퀘스트 정보를 Firebase에 저장
//void SavePlayerQuestsToFirebase()
//{
//    var auth = FirebaseManager.Instance.Auth;
//    var db = FirebaseManager.Instance.Db;
//    var user = auth.CurrentUser;
//    CollectionReference questCollection = db.Collection("Users").Document(user.UserId).Collection("Quests");

//    // 기존 퀘스트 문서 삭제
//    questCollection.GetSnapshotAsync().ContinueWithOnMainThread(deleteTask =>
//    {
//        if (deleteTask.IsFaulted)
//        {
//            Debug.LogError("기존 퀘스트 삭제 실패: " + deleteTask.Exception);
//        }
//        else
//        {
//            QuerySnapshot snapshot = deleteTask.Result;
//            foreach (DocumentSnapshot doc in snapshot.Documents)
//            {
//                doc.Reference.DeleteAsync();
//            }

//            // 현재 남아있는 퀘스트 정보를 저장
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

//                // quest.id를 문서 ID로 사용
//                DocumentReference questDoc = questCollection.Document(quest.id);
//                questDoc.SetAsync(questData).ContinueWithOnMainThread(writeTask =>
//                {
//                    if (writeTask.IsFaulted)
//                    {
//                        Debug.LogError("퀘스트 저장 실패: " + writeTask.Exception);
//                    }
//                    else
//                    {
//                        Debug.Log("퀘스트 저장 완료: " + quest.title);
//                    }
//                });
//            }
//        }
//    });
//}