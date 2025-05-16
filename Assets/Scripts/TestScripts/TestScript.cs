using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] GameObject testCube;
    [SerializeField] Transform child;


    private void Start()
    {
        GameObject go = PhotonNetwork.Instantiate($"Prefabs/{testCube.name}", transform.position, Quaternion.identity);
    }

}
