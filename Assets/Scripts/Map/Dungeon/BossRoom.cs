using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] public List<MonsterRandomSpawner_M> bossSpawnList;
    [SerializeField] public Transform playerSpawnPoint;
    [SerializeField] private Transform potalSpawnPoint;
    [SerializeField] private GameObject escapePotalPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnBossMonster()
    {
        foreach (MonsterRandomSpawner_M m in bossSpawnList)
        {
            m.SetSpawn();
        }
    }

    public void SpawnEscapePotal()
    {
        Instantiate(escapePotalPrefab).transform.position = potalSpawnPoint.position;

    }
}
