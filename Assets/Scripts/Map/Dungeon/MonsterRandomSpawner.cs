using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRandomSpawner : MonoBehaviour
{
    [SerializeField] SerializableArray<GameObject, float> monsterSpawnArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpawn()
    {
        if(SceneController.Instance.GetCurrentSceneName() == "MultiPlayTestScene")
        {
            //MultiplayScene
            foreach(var dictionary in monsterSpawnArray)
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                if(randomValue <= dictionary.Value) { PhotonNetwork.Instantiate($"Prefabs/Enemys/Multiplay/{dictionary.Key.name}", transform.position, Quaternion.identity); }
            }
        }
        else
        {
            //SingleplayScene
            foreach (var dictionary in monsterSpawnArray)
            {
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                //if (randomValue <= dictionary.Value) {Instantiate($"Prefabs/Enemys/Singleplay/{dictionary.Key.name}", transform.position, Quaternion.identity); }
            }
        }
    }
}
