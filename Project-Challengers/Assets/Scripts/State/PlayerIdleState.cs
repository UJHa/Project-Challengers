using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State
{
    public override void UpdateState()
    {
        base.UpdateState();
        MoveInput();
        AttackInput();
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _cCharacter.MoveRequest(ChessCharacter.Direction.UP);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _cCharacter.MoveRequest(ChessCharacter.Direction.DOWN);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _cCharacter.MoveRequest(ChessCharacter.Direction.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _cCharacter.MoveRequest(ChessCharacter.Direction.RIGHT);
        }
    }

    private void AttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("space!!!");
            _cCharacter.SetState(ChessCharacter.eState.ATTACK);
        }
    }
}
