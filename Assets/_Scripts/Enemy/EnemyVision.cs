using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    Enemy e;
    public List<Enemy> allies = new List<Enemy>();
    public List<Enemy> confirmedDeath = new List<Enemy>();
    private void Start()
    {
        e = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        e.PlayerInSight(other);
        if (allies.Count > 0)
        {
            foreach (var ally in allies)
            {
                if (ally.isAlive == false && !confirmedDeath.Contains(ally))
                {
                    e.GunShotAlert();
                    confirmedDeath.Add(ally);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy friend = other.GetComponent<Enemy>();
        if (friend && friend != e)
        {
            allies.Add(friend);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy friend = other.GetComponent<Enemy>();
        if (friend)
        {
            allies.Remove(friend);
        }
    }

}
