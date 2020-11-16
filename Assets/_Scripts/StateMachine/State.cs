using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public virtual void OnStateEnter(Enemy e)
    {
    }

    public virtual void OnStateExit(Enemy e)
    {
    }

    public virtual void StateUpdate(Enemy e)
    {
    }

    public virtual void StateLateUpdate(Enemy e)
    {
    }

}
