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
        Debug.Log("atk UpdateState!!!");
        _cCharacter.AttackUpdate();
    }
}
