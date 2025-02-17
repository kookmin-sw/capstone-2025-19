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
        if (PhotonNetwork.IsMasterClient) // ������ Ŭ���̾�Ʈ��� ��� �ݿ�
        {
            UpdatePhoton(input);
            photonView.RPC("UpdatePhoton", RpcTarget.OthersBuffered, input);
            DebugText.Instance.Debug($"Master input.ID {input.ID}");
        }
        else // Ŭ���̾�Ʈ��� �����Ϳ��� ��û
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
        if (PhotonNetwork.IsMasterClient) // �����Ϳ��� ����
        {
            UpdatePhoton(newID);
            photonView.RPC("UpdatePhoton", RpcTarget.AllBuffered, newID);
        }
    }
}
