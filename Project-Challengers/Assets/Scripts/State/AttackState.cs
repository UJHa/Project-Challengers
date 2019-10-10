using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void UpdateState()
    {
        base.UpdateState();
        character.AttackUpdate();
    }
}
