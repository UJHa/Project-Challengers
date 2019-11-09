using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void StartState()
    {
        base.StartState();
        Debug.Log("atk StartState!!!");
        _cCharacter.AttackStart();
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
