using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class LevelUpBnt : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerStatusController.Instance.LevelUpStep1();
            Debug.Log("LevelUpStep1");
        }
    }
}
