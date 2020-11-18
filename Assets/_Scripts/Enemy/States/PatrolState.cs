using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.searchBox.transform.localScale = e.viewDistancePatrol;
    }

    public override void OnStateExit(Enemy e)
    {
    }

    public override void StateUpdate(Enemy e)
    {
        if (e.playerInSight == true)
        {
            e.GunShotAlert();
        }
    }

    public override void StateLateUpdate(Enemy e)
    {
    }
}
