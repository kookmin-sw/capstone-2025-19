using System.Collections;
using System.Collections.Generic;
using PlayerControl;

using UnityEngine;

namespace PlayerControl
{
    public class PlayerAttacker : MonoBehaviour
    {
        int hashAttackCount = Animator.StringToHash("AttackCount");
        InputHandler input;
        Animator anim;
        // Start is called before the first frame update
        void Start()
        {
            input = GetComponent<InputHandler>();
            TryGetComponent(out anim);
        }

        // Update is called once per frame
        void Update()
        {
            if (input.attack && PlayerStatusController.Instance.canAttack)
            {
                input.attack = false;
                anim.SetTrigger("Attack");
                AttackCount = 0;
                PlayerStatusController.Instance.attack();
            }
        }

        public int AttackCount
        {
            get => anim.GetInteger(hashAttackCount);
            set => anim.SetInteger(hashAttackCount, value);
        }
    }
}

