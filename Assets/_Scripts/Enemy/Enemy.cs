using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public enum EnemyState {patrol, alert, investigating, cover}
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
        if (currentState == EnemyState.patrol)
        {
            currentState = EnemyState.cover;
            StartCoroutine(SearchCover());
            anim.SetTrigger("EnterCombat");
        }
        if (currentState == EnemyState.alert)
        {
            var lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 80);
        }
    }

    IEnumerator SearchCover()
    {
        Cover[] availableCovers = FindObjectsOfType<Cover>();
        Cover newCover = availableCovers[Random.Range(0, availableCovers.Length)];
        for (int i = 0; i < availableCovers.Length; i++)
        {
            if (availableCovers[i].taken == false)
            {
                break;
            }
            if (availableCovers[availableCovers.Length - 1].taken == true)
            {
                Debug.Log("all covers are taken");
                currentState = EnemyState.alert;
                yield break;
            }
        }
        while (newCover.taken)
        {
            newCover = availableCovers[Random.Range(0, availableCovers.Length)];
            yield return new WaitForSeconds(0.1f);
        }
        newCover.taken = true;

        Vector3 newPos = newCover.transform.position;
        agent.SetDestination(newPos);
        anim.SetTrigger("GoToCover");

        while (Vector3.Distance(transform.position, newPos) > 0.75f)
        {
            yield return new WaitForEndOfFrame();
            //Debug.Log(Vector3.Distance(transform.position, newPos));
        }
        anim.SetTrigger("ExitCover");
        currentState = EnemyState.alert;
    }

    void Patrol()
    {
        
    }

    void Alert()
    {
        Vector3 startPos = transform.position + Vector3.up;
        Vector3 direction = player.position - startPos;
        Ray scan = new Ray(startPos, direction);
        RaycastHit hit;
        Debug.DrawRay(startPos, direction * 2, Color.blue);
        if (Physics.Raycast(scan, out hit, 10))
        {
            if (hit.collider.CompareTag("Humanoid"))
            {
                FPSPlayer player = hit.collider.GetComponent<FPSPlayer>();
                if (player)
                {
                    var lookPos = player.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
                }
            }
        }
    }

    void Investigating()
    {

    }

}
