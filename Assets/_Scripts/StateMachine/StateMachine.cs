using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State currentState;
    public List<State> allStates = new List<State>();

    public void GoToState(Enemy e, string newstate)
    {
        if (currentState != null)
        {
            currentState.OnStateExit(e);
        }
        foreach (var s in allStates)
        {
            if (s.GetType().ToString() == newstate)
            {
                currentState = s;

                e.ChangeState(currentState);

                s.OnStateEnter(e);
                Debug.Log("Changed state to " + currentState.GetType().ToString());
                return;
            }
        }
        Debug.LogError("State " + "'" + newstate + "'" + " doesnt exist");
    }

    public State CurrentState()
    {
        return currentState;
    }

    public bool IsInState(string state)
    {
        if (state == currentState.GetType().ToString())
        {
            return true;
        }
        return false;
    }
}
