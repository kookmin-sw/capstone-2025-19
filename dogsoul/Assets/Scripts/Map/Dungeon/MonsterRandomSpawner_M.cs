using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MonsterRandomSpawner_M : MonsterRandomSpawner
{
    public override void SetSpawn()
    {
        //MultiplayScene
        foreach (var dictionary in monsterSpawnArray)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            if (randomValue <= dictionary.Value) { PhotonNetwork.Instantiate($"Prefabs/Enemys/Multiplay/{dictionary.Key.name}", transform.position, Quaternion.identity); }
        }
    }

}
