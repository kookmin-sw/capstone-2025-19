using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractGo
{
    [SerializeField] private Animator animator;
    private bool isOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    

    public override void InteractObject()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }

    public override void CloseInteract()
    {
        //TODO Nothing. maybe remove interact UI
    }
}
