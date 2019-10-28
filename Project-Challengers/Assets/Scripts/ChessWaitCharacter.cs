using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessWaitCharacter : ChessCharacter
{
    protected override void InitState()
    {
        Debug.Log(this.name + " : InitState");
        SetState(eState.IDLE);
        _prevState = eState.IDLE;

        stateMap = new Dictionary<eState, State>();
        stateMap[eState.IDLE] = new IdleState();
        stateMap[eState.MOVE] = new IdleState();
        stateMap[eState.ATTACK] = new IdleState();
        stateMap[eState.DEAD] = new IdleState();

        for (eState i = 0; i < eState.MAXSIZE; i++)
        {
            stateMap[i].InitState(this);
        }
        Debug.Log(this.name + " : InitState 2");
    }
}
