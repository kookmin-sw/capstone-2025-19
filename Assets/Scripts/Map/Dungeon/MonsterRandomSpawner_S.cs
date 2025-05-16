using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRandomSpawner_S : MonsterRandomSpawner
{
    public override void SetSpawn()
    {
        //SingleplayScene
        foreach (var dictionary in monsterSpawnArray)
        {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            //if (randomValue <= dictionary.Value) {Instantiate($"Prefabs/Enemys/Singleplay/{dictionary.Key.name}", transform.position, Quaternion.identity); }
        }
    }


}
