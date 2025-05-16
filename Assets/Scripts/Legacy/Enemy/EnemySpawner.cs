using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks {
    public GameObject enemyPrefab;

    private void Start() {
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
    }
    
}