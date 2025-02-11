using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestClass : MonoBehaviour
{
    public TestID testID = new TestID();

    PhotonView photonView;

    public int id = 0;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpdate(TestID input)
    {
        Debug.Log("test");
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트라면 즉시 반영
        {
            UpdatePhoton(input);
            photonView.RPC("UpdatePhoton", RpcTarget.OthersBuffered, input);
            DebugText.Instance.Debug($"Master input.ID {input.ID}");
        }
        else // 클라이언트라면 마스터에게 요청
        {
            photonView.RPC("RequestUpdatePhoton", RpcTarget.MasterClient, input);
            DebugText.Instance.Debug($"client input.ID {input.ID}");
        }
        
    }
    [PunRPC]
    private void UpdatePhoton(TestID input)
    {
        testID = input;
        this.id = testID.ID;
        DebugText.Instance.Debug($"[RPC] TestID is Update {testID.ID}");
        
    }
    [PunRPC]
    private void RequestUpdatePhoton(TestID newID)
    {
        if (PhotonNetwork.IsMasterClient) // 마스터에서 실행
        {
            UpdatePhoton(newID);
            photonView.RPC("UpdatePhoton", RpcTarget.AllBuffered, newID);
        }
    }
}
