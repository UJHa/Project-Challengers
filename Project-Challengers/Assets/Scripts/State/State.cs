﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected ChessCharacter _cCharacter;

    public State()
    {

    }

    public virtual void InitState(ChessCharacter cCharacter)
    {
        _cCharacter = cCharacter;
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
