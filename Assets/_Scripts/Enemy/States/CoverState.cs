using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.playerLastPosition = e.player.position;
        e.anim.SetTrigger("GoToCover");
        e.agent.speed = 3f;
    }

    public override void OnStateExit(Enemy e)
    {
        e.anim.SetTrigger("ExitCover");
    }

    public override void StateUpdate(Enemy e)
    {
    }

    public override void StateLateUpdate(Enemy e)
    {
    }
}
