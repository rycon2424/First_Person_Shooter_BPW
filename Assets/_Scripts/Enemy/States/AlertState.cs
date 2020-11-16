using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : State
{
    public override void OnStateEnter(Enemy e)
    {
        e.searchBox.transform.localScale = e.viewDistanceAlert;
        e.playerLastPosition = e.player.position;
        e.searchBox.enabled = false;
        e.anim.SetBool("Walking", false);
        e.agent.SetDestination(e.transform.position);
        e.anim.SetTrigger("EnterCombat");
    }

    public override void OnStateExit(Enemy e)
    {
    }
    
    public override void StateUpdate(Enemy e)
    {
        GameObject target = e.SeeActor(e.transform.position + e.eyeOffset);
        if (target)
        {
            e.tempPlayer = target.GetComponent<FPSPlayer>();
            if (e.tempPlayer)
            {
                e.waitingForPatience = false;
                StartShooting(e);
            }
        }
        else
        {
            e.shooting = false;
            e.tempPlayer = null;
            if (e.waitingForPatience == false)
            {
                e.StartRoutine("Patience");
                e.waitingForPatience = true;
            }
        }
    }

    void StartShooting(Enemy e)
    {
        e.playerLastPosition = e.player.position;

        var lookPos = e.tempPlayer.transform.position - e.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        e.transform.rotation = Quaternion.Slerp(e.transform.rotation, rotation, Time.deltaTime * 5);

        if (e.weapon.cooldown == false)
        {
            if (e.shooting == false)
            {
                e.StartRoutine("ReactionShoot");
            }
            else
            {
                e.weapon.transform.LookAt(e.player);
                e.weapon.Shoot();
            }
        }
    }

    public override void StateLateUpdate(Enemy e)
    {
    }
}
