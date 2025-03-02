using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingBladeTrigger : MonoBehaviour
{

    [SerializeField] private int damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        // �±װ� "Player"�� ������Ʈ�� Ʈ���ſ� ����
        if (other.CompareTag("Player"))
        {
            Debug.Log("player�� swingblade ������ �ɸ�");
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
