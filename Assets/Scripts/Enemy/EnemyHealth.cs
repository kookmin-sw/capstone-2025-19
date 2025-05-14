using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{

    protected Animator animator;
    protected EnemyState enemyState;
    public Slider hpBar;
    public int enemyExp;
    protected PhotonView photonView;
    [SerializeField] protected float removeTime = 5.0f;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        enemyState = GetComponent<EnemyState>();
        photonView = GetComponent<PhotonView>();
    }
    protected void UpdateHpBar()
    {
        hpBar.value = currentHealth / maxHealth;
    }

    public virtual void TakeDamage(float damage, DamageCollider attackerWeapon, Vector3 contactPos, ParticleSystem hitEffect)
    {
        DamageCollider myWeaponCollider = GetComponentInChildren<DamageCollider>();
        
        #region Escape
        // when Enemy is invincible
        if (enemyState.state == EnemyState.State.Invincible || enemyState.state == EnemyState.State.Die) return;

        if (attackerWeapon != null && myWeaponCollider !=null)
        {
            if (animator.GetBool("Attacking") &&(myWeaponCollider.tenacity > attackerWeapon.tenacity) ) return;
        }
        #endregion

        #region Hit
        currentHealth -= damage;
        UpdateHpBar();

        if (myWeaponCollider != null)
        {
            myWeaponCollider.dontOpenCollider = true;
            if (myWeaponCollider.damageCollider.enabled) myWeaponCollider.UnableDamageCollider();
        }

        if (hitEffect != null)
        {
            StartCoroutine(WaitForParticleEnd(hitEffect, contactPos));
        }
    
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == "Hit")
            {
                animator.SetTrigger("Hit");
                enemyState.state = EnemyState.State.Invincible;
                break;
            }
        }
        #endregion

        // die
        if (currentHealth <= 0)
        {
            enemyState.ChangeState(EnemyState.State.Die);
            photonView.RPC(nameof(ReceiveDieState), RpcTarget.OthersBuffered);
        }
    }

    public virtual void DieActive()
    {
        animator.ResetTrigger("Hit");
        currentHealth = 0;
        animator.SetTrigger("Die");
        GetComponent<EnemyController>().DeathTrigger();

        Transform player = this.GetComponent<EnemyController>().target;
        player.GetComponent<PlayerGetStatus>().GetExpFromEnemy(enemyExp);
        StartCoroutine(RemoveMonsterBody());
    }

    protected IEnumerator WaitForParticleEnd(ParticleSystem particle, Vector3 position)
    {
        ParticleSystem ps = Instantiate(particle, position, Quaternion.identity);
        ps.Play(); 

        while (ps.IsAlive(true))
        {
            yield return null;
        }

        Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    protected IEnumerator RemoveMonsterBody()
    {
        yield return new WaitForSeconds(removeTime);
        Destroy(gameObject);
    }

    [PunRPC]
    private void ReceiveDieState()
    {
        enemyState.ChangeState(EnemyState.State.Die);
    }

}
