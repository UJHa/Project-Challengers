using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Knight character;

    public State()
    {

    }

    public virtual void UpdateState()
    {

    }

    public void SetCharacter(Knight knight)
    {
        character = knight;
    }
}
