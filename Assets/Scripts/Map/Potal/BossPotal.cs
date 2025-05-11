using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPotal : MonoBehaviour
{
    protected void OnTriggerEnter(Collider other)
    {
        EnterBossRoom();
    }

    protected virtual void EnterBossRoom()
    {
        DungeonGenerator.Instance.EnterBossRoom();
    }
}
