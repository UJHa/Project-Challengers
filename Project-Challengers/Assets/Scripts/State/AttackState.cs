﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("atk StartState!!!");
        _cCharacter.AttackStart();
        Vector3 directionVector = _cCharacter.GetAttackTarget().transform.position - _cCharacter.transform.position;
        if (directionVector.x < 0.0f)
        {
            _cCharacter.SetDirection(ChessCharacter.Direction.LEFT);
        }
        else
        {
            _cCharacter.SetDirection(ChessCharacter.Direction.RIGHT);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _cCharacter.AttackUpdate();
    }

    public override void EndState()
    {
        base.EndState();
        Debug.Log("ATTack END!!!!!");
        _cCharacter.SetAttackTarget(null);
    }
}
