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
        
    }
}
