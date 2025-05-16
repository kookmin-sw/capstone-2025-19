using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass_Trigger : MonoBehaviour
{
    [SerializeField] GameObject arrow_trap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"인 오브젝트가 진입 & 아직 발사 안 했다면
        if (other.CompareTag("Player") && (!arrow_trap.GetComponent<Wall_Arrow>().triggered))
        {
            Debug.Log("풀에 캐릭터 들어옴");
            StartCoroutine(arrow_trap.GetComponent<Wall_Arrow>().FireArrowsRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
