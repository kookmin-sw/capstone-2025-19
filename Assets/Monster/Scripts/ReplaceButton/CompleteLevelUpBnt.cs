using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class CompleteLevelUpBnt : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatusController.Instance.PressPointDecomposeCompleteBnt();
            Debug.Log("PressPointDecomposeCompleteBnt");
        }
    }
}
