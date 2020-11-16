using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.anim.SetBool("Walking", true);
        e.agent.speed = 1.5f;
        e.agent.SetDestination(e.playerLastPosition);
        e.searchBox.enabled = true;
    }

    public override void OnStateExit(Enemy e)
    {
    }

    public override void StateUpdate(Enemy e)
    {
        if (Vector3.Distance(e.playerLastPosition, e.transform.position) < 1.5f)
        {
            e.anim.SetBool("Walking", false);
        }
    }

    public override void StateLateUpdate(Enemy e)
    {
    }
}
