﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public override void UpdateState()
    {
        base.UpdateState();
        character.MoveInput();
        character.AttackInput();
    }
}
