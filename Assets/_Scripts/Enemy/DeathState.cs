using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.anim.SetTrigger("Death");
        e.isAlive = false;
        e.GetComponent<CharacterController>().enabled = false;
        e.agent.SetDestination(e.transform.position);
    }

    public override void OnStateExit(Enemy e)
    {
    }

    public override void StateUpdate(Enemy e)
    {
    }

    public override void StateLateUpdate(Enemy e)
    {
    }
}
