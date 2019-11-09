using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : State
{
    public override void StartState()
    {
        base.StartState();
        _cCharacter.GetAnimator().SetBool("isMoving", true);
    }
    public override void UpdateState()
    {
        base.UpdateState();
        _cCharacter.transform.position = Vector3.MoveTowards(_cCharacter.transform.position, _cCharacter.GetTargetPosition(), _cCharacter.moveSpeed * Time.deltaTime);
        if (Vector3.Distance(_cCharacter.transform.position, _cCharacter.GetTargetPosition()) == 0.0f)
        {
            _cCharacter.SetState(ChessCharacter.eState.IDLE);
        }
    }
    public override void EndState()
    {
        base.EndState();
        _cCharacter.GetAnimator().SetBool("isMoving", false);
    }
}
