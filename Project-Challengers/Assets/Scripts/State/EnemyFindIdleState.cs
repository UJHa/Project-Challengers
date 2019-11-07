using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFindIdleState : State
{
    private float findTimer;

    public override void StartState()
    {
        base.StartState();
        findTimer = 0.0f;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (findTimer >= 1.0f) // 1초에 한번 씩 주변 범위 적 탐색
        {
            ChessCharacter targetCharacter = FindAdjacentTarget(3/*findRange*/);
            if (targetCharacter != null)
            {
                _cCharacter.SetTargetTilePosition(targetCharacter.GetTilePosition()/*타일 좌표 0,0~7,7사이*/);
                _cCharacter.SetState(ChessCharacter.eState.MOVE);
            }

            findTimer = 0.0f;
        }
        findTimer += Time.deltaTime;
    }

    public override void EndState()
    {
        base.EndState();
        findTimer = 0.0f;
    }

    private ChessCharacter FindAdjacentTarget(int findRange)
    {
        return null;
    }
}
