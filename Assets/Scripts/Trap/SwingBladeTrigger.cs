using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBladeTrigger : MonoBehaviour
{

    [SerializeField] private int damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"인 오브젝트가 트리거에 진입
        if (other.CompareTag("Player"))
        {
            Debug.Log("player가 swingblade 함정에 걸림");
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
