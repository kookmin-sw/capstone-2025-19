using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;

    private int animIDHit;
    private int animIDDie;
    private int animIDInteracting;
    private int animIDBlocking;

    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        animIDHit = Animator.StringToHash("Hit");
        animIDDie = Animator.StringToHash("Die");


    }

    void Start()
    {
        maxHealth = SetHealthFromHealthLevel();
        currentHealth = maxHealth;
    }

    private int SetHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger(animIDHit);
        if (gameObject.tag == "Player")
        {
            animator.SetBool(animIDInteracting, true);
            animator.SetBool(animIDBlocking, true);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.SetTrigger(animIDDie);
        }
    }
}
