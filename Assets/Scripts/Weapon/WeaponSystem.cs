using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    public Item weaponItem;
    protected void Awake()
    {
        weaponAnimator = GetComponent<Animator>();
    }

    abstract public void Attack();
    // Start is called before the first frame update
    
}
