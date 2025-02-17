using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.QuestCanvas.SetActive(false);
    }

    //캐릭터가 퀘스트 보드에 가까이 갔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager.Instance.QuestCanvas.SetActive(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager.Instance.QuestCanvas.SetActive(false);
        }
    }


}
