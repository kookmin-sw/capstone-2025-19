using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public List<InteractGo> interactGoList = new List<InteractGo>();
    public Transform dropItemPosition;

    public bool eType = false;
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
        if (other.CompareTag("Interact"))
        {
            InteractGo interact = other.GetComponent<InteractGo>();
            if (!interactGoList.Contains(interact))
            {
                interactGoList.Add(interact);
            }
            if(other.TryGetComponent<DropItem>(out DropItem dropItem))
            {
                InventoryController.Instance.EnterDropItem(dropItem);
            }
        }
        /*if (other.CompareTag("DropItem"))
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

        }else if (other.CompareTag("Money"))
        {
            InventoryController.Instance.SetMoneyItemIcon(other.GetComponent<MoneyDropItem>());
        }else if (other.CompareTag("Trigger"))
        {
            if (!interactGoList.Contains(other.GetComponent<InteractGo>()))
            {
                interactGoList.Add(other.GetComponent<InteractGo>());
            }
        }*/
        if(other.TryGetComponent<TriggerUI>(out TriggerUI triggerUI))
        {
            Debug.Log("1");

            triggerUI.StartEvent();
        }
    }
    //Function Player dectected objects
    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"other is {other.gameObject.name}");
        
        if (other.CompareTag("Interact"))
        {
            InteractGo interact = other.GetComponent<InteractGo>();
            if (interactGoList.Contains(interact))
            {
                interactGoList.Remove(interact);
            }
            if(other.TryGetComponent<DropItem>(out DropItem dropItem))
            {
                Debug.Log("왜 두번 실행하는거야");
                InventoryController.Instance.ExitDropItem(dropItem);
            }
        }
       /* //DebugText.Instance.Debug("trigger exit test");
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

        }else if (other.CompareTag("Money"))
        {
            InventoryController.Instance.RemoveMoneyItemIcon(other.GetComponent<MoneyDropItem>());
        }
        else if (other.CompareTag("Trigger"))
        {
            if (interactGoList.Contains(other.GetComponent<InteractGo>())) { interactGoList.Remove(other.GetComponent<InteractGo>()); }
        }
        if (other.TryGetComponent<TriggerUI>(out TriggerUI triggerUI))
        {
            triggerUI.EndEvent();
        }*/
    }

    public void InteractObject()
    {
        Debug.Log($"trigger test {interactGoList.Count}");
        if(interactGoList.Count > 0)
        {
            InteractGo go = interactGoList[0];
            go.InteractObject();
        }

        /*if (InventoryController.Instance.dropItemList.Count > 0)
        {
            DropItem item = InventoryController.Instance.dropItemList[0];
            InventoryController.Instance.PickupDropItem(item);
        }
        else
        {
            //TODO Use InteractObject ex) Open the door, Open the chest, Insert Potal
            
        }*/

    }
}
