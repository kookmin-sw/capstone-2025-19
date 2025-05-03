using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController_S : BossController
{
    public override void ThrowRock()
    {
        Instantiate(rockPrefab, rockInitPos);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
