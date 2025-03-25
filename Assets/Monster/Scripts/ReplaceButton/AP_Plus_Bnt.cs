using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class AP_Plus_Bnt : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatusController.Instance.ApPlusButton();
            Debug.Log("ApPlusButton");
        }
    }
}
