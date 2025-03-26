using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AP_Minus_Bnt : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatusController.Instance.ApMinusButton();
            Debug.Log("ApMinusButton");
        }
    }
}
