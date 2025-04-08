using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPositionList;
    [SerializeField] SerializableArray<GameObject, float> testArray;
    [SerializeField] GameObject testPrefab;
    bool testbool = false;
    TestReceive receive;
    int testInt = 4;

    public enum NetworkEventCode : byte
    {
        Test1 = 1,
        Test2 = 2,
        Test3 = 3,
    }

    public void Start()
    {
        receive = GetComponent<TestReceive>();
        if (PhotonNetwork.IsMasterClient)
        {
            StartFucntion();
        }
        else
        {
            RequestPhoton();
        }
        Debug.Log("MultiPlay test");
    }

    private void StartFucntion()
    {
        foreach(var test in testArray)
        {
            if(UnityEngine.Random.Range(0, 1) > test.Value)
            {
                GameObject go = PhotonNetwork.Instantiate($"Prefabs/TestPrefab/{test.Key.name}", transform.position, Quaternion.identity);
                Debug.Log(go.name);
            }
        }
        testbool = true;
        Debug.Log("Loop finish");
    }

    private void RequestPhoton()
    {
        PhotonNetwork.RaiseEvent((byte)TestScript.NetworkEventCode.Test1, testInt, 
            new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.MasterClient }, 
            SendOptions.SendReliable);
    }

    public void SetSpawnPosition(Vector3 position_)
    {
        GameObject go = PhotonNetwork.Instantiate($"Prefabs/TestPrefab/{testPrefab.name}", position_ , Quaternion.identity);
    }
    public void SendPosition(int index)
    {
        string testText = "testText";
        object[] data = new object[] { testText, spawnPositionList[index].position };
        PhotonNetwork.RaiseEvent((byte)TestScript.NetworkEventCode.Test2, data, new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.All },
            SendOptions.SendReliable);
    }
    
}
