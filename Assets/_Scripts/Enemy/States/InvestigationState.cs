﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.anim.SetBool("Walking", true);
        e.agent.speed = 1.5f;
        e.agent.SetDestination(e.playerLastPosition);
    }

    public override void OnStateExit(Enemy e)
    {
    }

    public override void StateUpdate(Enemy e)
    {
        if (Vector3.Distance(e.playerLastPosition, e.transform.position) < 2)
        {
            e.anim.SetBool("Walking", false);
        }
        if (e.playerInSight == true)
        {
            e.GunShotAlert();
        }
    }

    public override void StateLateUpdate(Enemy e)
    {

    }
}
