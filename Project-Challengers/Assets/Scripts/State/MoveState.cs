using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    public override void UpdateState()
    {
        base.UpdateState();
        character.MoveUpdate();
    }
}
