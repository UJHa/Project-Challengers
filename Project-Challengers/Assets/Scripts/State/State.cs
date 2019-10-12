using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected ChessCharacter _cCharacter;
    private ChessCharacter.eState currentState;

    public State()
    {

    }

    public void InitState(ChessCharacter cCharacter)
    {
        _cCharacter = cCharacter;
        currentState = _cCharacter.GetState();
    }

    public virtual void StartState()
    {

    }

    public virtual void UpdateState()
    {
        
    }

    public virtual void EndState()
    {
        
    }
}
