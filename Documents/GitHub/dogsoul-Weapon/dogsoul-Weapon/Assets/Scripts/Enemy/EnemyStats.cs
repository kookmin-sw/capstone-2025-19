using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int currentHealth;


    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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
        animator.Play("Hit");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            animator.Play("Death");
        }
    }
}
