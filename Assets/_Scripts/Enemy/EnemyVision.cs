using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    Enemy e;
    private void Start()
    {
        e = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        e.PlayerInSight(other);
    }

}
