using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public enum EnemyState {patrol, alert, investigating}
    public EnemyState currentState;

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<FPSPlayer>().transform;
    }
    
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.patrol:
                Patrol();
                break;
            case EnemyState.alert:
                Alert();
                break;
            case EnemyState.investigating:
                Investigating();
                break;
            default:
                break;
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (currentState != EnemyState.alert)
        {
            currentState = EnemyState.alert;
            StartCoroutine(SearchCover());
            anim.SetTrigger("EnterCombat");
        }
    }

    IEnumerator SearchCover()
    {
        Cover[] availableCovers = FindObjectsOfType<Cover>();
        Cover newCover = availableCovers[Random.Range(0, availableCovers.Length)];
        while (newCover.taken)
        {
            newCover = availableCovers[Random.Range(0, availableCovers.Length)];
            yield return new WaitForSeconds(0.5f);
        }
        newCover.taken = true;

        Vector3 newPos = newCover.transform.position;
        agent.SetDestination(newPos);
        anim.SetTrigger("GoToCover");

        while (Vector3.Distance(transform.position, newPos) > 0.75f)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log(Vector3.Distance(transform.position, newPos));
        }
        anim.SetTrigger("ExitCover");
    }

    void Patrol()
    {
        
    }

    void Alert()
    {

    }

    void Investigating()
    {

    }

}
