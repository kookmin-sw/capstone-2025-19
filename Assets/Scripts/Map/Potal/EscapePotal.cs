using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EscapePotal : MonoBehaviour
{
    public bool isUse = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO Enter Vileage
            NetworkController.Instance.EnterVillage();

        }
    }
}
