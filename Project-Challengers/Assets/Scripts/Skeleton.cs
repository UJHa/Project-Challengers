using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : ChessCharacter
{
    protected override void InitData()
    {
        base.InitData();

        animator.SetBool("isAttack", false);
    }

    public override void AttackStart()
    {
        animator.SetBool("isAttack", true);
    }

    public override void AttackUpdate()
    {
        if (true)
        {
            //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).length);

            Debug.Log(animator.GetNextAnimatorStateInfo(0).normalizedTime);
            if (animator.GetNextAnimatorStateInfo(0).normalizedTime > .1f)
            {
                animator.SetBool("isAttack", false);
                SetState(eState.IDLE);
            }
            //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            //Debug.Log(animator.GetNextAnimatorStateInfo(0).normalizedTime);
            //animator.SetBool("isAttack", false);
            //SetState(eState.IDLE);
        }
    }
}
