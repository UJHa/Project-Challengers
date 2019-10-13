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
            Debug.Log(direction);
            if (_cCharacter.CanMoveTile(direction))
            {
                _cCharacter.MoveRequest(direction);
            }
            else
            {
                //이동 불가 원인에 따라 대기, 공격 등 구현 예정
                _cCharacter.SetState(ChessCharacter.eState.ATTACK);
            }
            moveTimer = 0.0f;
        }
        moveTimer += Time.deltaTime;
    }
}
