using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReceive : MonoBehaviour
{
    
    /*TestScript testScript;
    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
    Dictionary<byte, Action<EventData>> handlers;
    // Start is called before the first frame update
    void Start()
    {
        handlers = new Dictionary<byte, Action<EventData>>
        {
            {(byte)TestScript.NetworkEventCode.Test1, Test1Receive }
        };
        testScript = GetComponent<TestScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestPlayerSpawn()
    {

    }

    public void OnEvent(EventData photonEvent)
    {
        if(handlers.TryGetValue(photonEvent.Code, out var handler_)) handler_.Invoke(photonEvent);
    }

    private void Test1Receive(EventData photonEvent)
    {
        if (PhotonNetwork.IsMasterClient) { testScript.SendPosition((int)photonEvent.CustomData); }
    }*/
}
