using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : Singleton<MonsterController>
{
    protected override void Awake()
    {
        base.Awake();
        SceneController.Instance.SubscribeGo(this.gameObject);
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
