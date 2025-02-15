using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    [SerializeField] GameObject QuestCanvas;

    // Start is called before the first frame update
    void Start()
    {
        QuestCanvas.SetActive(false);
    }

    //ĳ���Ͱ� ����Ʈ ���忡 ������ ���� ��
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestCanvas.SetActive(true);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestCanvas.SetActive(false);
        }
    }


}
