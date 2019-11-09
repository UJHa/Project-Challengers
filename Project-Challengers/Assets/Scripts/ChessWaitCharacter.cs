using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessWaitCharacter : ChessCharacter
{
    protected override void InitData()
    {
        base.InitData();
        characterType = eCharacterType.WAIT;
    }
    protected override void InitState()
    {
        //Debug.Log(this.name + " : InitState");
        SetState(eState.IDLE);
        _prevState = eState.IDLE;

        stateMap = new Dictionary<eState, State>();
        stateMap[eState.IDLE] = new WaitIdleState();
        stateMap[eState.MOVE] = new WaitIdleState();
        stateMap[eState.ATTACK] = new WaitIdleState();
        stateMap[eState.DEAD] = new WaitIdleState();

        for (eState i = 0; i < eState.MAXSIZE; i++)
        {
            stateMap[i].InitState(this);
        }
    }
}
