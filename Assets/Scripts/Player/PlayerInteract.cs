using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    List<GameObject> interactGOs = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            interactGOs.Add(other.gameObject);
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractiveObject"))
        {
            InteractGo interact = other.GetComponent<InteractGo>();
            interact.CloseInteract();
            interact.enabled = false;
            interactGOs.Remove(other.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        //TODO player의 상태가 Idle일 경우
        if (Input.GetKeyDown(KeyCode.E) && interactGOs.Count != 0)
        {
            
            GameObject curObject = interactGOs[0];
            InteractGo interact = curObject.GetComponent<InteractGo>();
            interact.enabled=true;
            interact.InteractObject();
            interactGOs.Remove(curObject);
            interactGOs.Add(curObject);
        }
    }


}
