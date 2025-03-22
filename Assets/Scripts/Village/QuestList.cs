using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestList : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI content;
    public Quest Quest;

    //game scene에서 list에서 quest를 선택했을 때 실행.
    public void showQuest()
    {
        QuestManager.Instance.setCurrentQuestList(this);

        if(this.gameObject.transform.parent.name == "PlayerQuestListPanel")
        {
            QuestManager.Instance.acceptButton.SetActive(false);
            QuestManager.Instance.dropButton.SetActive(true);

            if (Quest.isCompleted)
            {
                QuestManager.Instance.dropButton.SetActive(false);
                QuestManager.Instance.rewardButton.SetActive(true);
            }
        }
        else
        {
            QuestManager.Instance.dropButton.SetActive(false);
            QuestManager.Instance.acceptButton.SetActive(true);
            QuestManager.Instance.rewardButton.SetActive(false);
        }
    }

    public void createQuest(Quest quest)
    {
        this.Quest = quest;
        this.title.text = quest.title;
        this.content.text = QuestManager.Instance.generateContent(quest);
    }

    
}
