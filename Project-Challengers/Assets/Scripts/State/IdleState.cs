using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    private float moveTimer;

    public override void StartState()
    {
        base.StartState();
        moveTimer = 0.0f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (moveTimer >= 1.0f)
        {
            ChessCharacter.Direction direction = (ChessCharacter.Direction)Random.Range(0, 4);
            _cCharacter.MoveRequest(direction);
            moveTimer = 0.0f;
        }
        moveTimer += Time.deltaTime;
    }

    public override void EndState()
    {
        base.EndState();
        moveTimer = 0.0f;
    }
}
