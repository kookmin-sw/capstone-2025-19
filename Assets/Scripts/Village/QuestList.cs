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

    public void showQuest()
    {
        QuestManager.Instance.setCurrentQuestList(this);

        if(this.gameObject.transform.parent.name == "PlayerQuestListPanel")
        {
            QuestManager.Instance.acceptButton.SetActive(false);
            QuestManager.Instance.dropButton.SetActive(true);
        }
        else
        {
            QuestManager.Instance.dropButton.SetActive(false);
            QuestManager.Instance.acceptButton.SetActive(true);
        }
    }

    public void setQuest(Quest quest)
    {
        this.Quest = quest;
        this.title.text = quest.title;
        this.content.text = QuestManager.Instance.generateContent(quest);
    }


}
