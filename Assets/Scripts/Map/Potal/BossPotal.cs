using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPotal : MonoBehaviour
{
    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("potal test");
            EnterBossRoom();
        }
    }

    protected virtual void EnterBossRoom()
    {
        Debug.Log("potal test 1");
        DungeonGenerator.Instance.EnterBossRoom();
    }
}
