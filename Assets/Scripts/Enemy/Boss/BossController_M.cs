using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BossController_M : BossController
{
    
    protected override void Awake()
    {
        base.Awake();
    }

    public override void ThrowRock()
    {
        PhotonNetwork.Instantiate($"Prefabs/Enemys/Multiplay/Rock", rockInitPos.position, Quaternion.identity);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void DieData()
    {


    }

    
}
