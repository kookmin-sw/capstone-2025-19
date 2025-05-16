using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MonsterRandomSpawner : MonoBehaviour
{
    [SerializeField] protected SerializableArray<GameObject, float> monsterSpawnArray;

    abstract public void SetSpawn();
    
}
