using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [Header("Actor Stats")]
    public int health;
    public bool isAlive = true;
    [Header("FootSteps")]
    public AudioClip[] footsteps;
    public AudioSource footSource;

    public virtual void TakeDamage(int damage)
    {
        if (isAlive == false)
        {
            return;
        }
        health -= damage;
    }

    public void FootSteps()
    {
        footSource.clip = footsteps[Random.Range(0, footsteps.Length)];
        footSource.Play();
    }
}
