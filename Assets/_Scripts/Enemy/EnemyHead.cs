using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : Actor
{
    public Enemy e;
    public override void TakeDamage(int damage)
    {
        e.TakeDamage(damage * 10);
    }
}
