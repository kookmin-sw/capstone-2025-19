using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public Transform dropItemPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"update position {transform.position}");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropItem"))
        {

            //TODO 인벤토리에 ItemIcon넣기
            if (other.GetComponent<DropItem>() == null)
            {
                InventoryController.Instance.EnterDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                InventoryController.Instance.EnterDropItem(other.GetComponent<DropItem>());
            }

        }
        if(other.TryGetComponent<TriggerUI>(out TriggerUI triggerUI))
        {
            Debug.Log("1");

            triggerUI.StartEvent();
        }

        if (other.CompareTag("Monster"))
        {
            Debug.Log("Hit by Monster");
            //맞는 애니메이션 실행
            //플레이어 체력 조정
            int damage = other.GetComponent<MonsterStatus>().damage;
            PlayerStatusController.Instance.getDamage(damage);
        }
    }
    //Function Player dectected objects
    private void OnTriggerExit(Collider other)
    {
        //DebugText.Instance.Debug("trigger exit test");
        if (other.CompareTag("DropItem"))
        {
            //TODO 인벤토리에 ItemIcon 빼기
            if (other.GetComponent<DropItem>() == null)
            {
                InventoryController.Instance.ExitDropItem(other.transform.parent.GetComponent<DropItem>());
            }
            else
            {
                InventoryController.Instance.ExitDropItem(other.GetComponent<DropItem>());
            }

        }
        if (other.TryGetComponent<TriggerUI>(out TriggerUI triggerUI))
        {
            triggerUI.EndEvent();
        }
    }

}
