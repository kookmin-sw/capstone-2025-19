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
        // �±װ� "Player"�� ������Ʈ�� ���� & ���� �߻� �� �ߴٸ�
        if (other.CompareTag("Player") && (!arrow_trap.GetComponent<Wall_Arrow>().triggered))
        {
            Debug.Log("Ǯ�� ĳ���� ����");
            StartCoroutine(arrow_trap.GetComponent<Wall_Arrow>().FireArrowsRoutine());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
