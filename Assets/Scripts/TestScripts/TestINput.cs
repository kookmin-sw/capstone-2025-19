using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestINput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Keyboard input");
        }
    }
}
