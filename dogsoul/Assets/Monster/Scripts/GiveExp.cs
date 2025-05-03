using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveExp : MonoBehaviour
{
    [SerializeField] int exp = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatusController.Instance.getExp(exp);
            Debug.Log("경험치를 얻음");
        }
    }
}
