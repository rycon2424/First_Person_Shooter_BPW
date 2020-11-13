using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int health;
    public bool isAlive = true;

    public virtual void TakeDamage(int damage)
    {
        if (isAlive == false)
        {
            return;
        }
        health -= damage;
    }
}
