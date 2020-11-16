using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public enum EnemyState { patrol, alert, investigating, cover }
    public EnemyState currentState;
    [Space]
    [Range(0, 100)] public int coverChance;
    public float reactionTime;
    public float maxPatience;
    [Space]
    public Weapon weapon;

    private Vector3 playerLastPosition;
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private FPSPlayer fpsPlayer;
    private bool shooting;
    private bool waitingForPatience;
    private MeshCollider searchBox;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        fpsPlayer = FindObjectOfType<FPSPlayer>();
        player = fpsPlayer.transform;
        searchBox = GetComponentInChildren<MeshCollider>();
    }

    void Update()
    {
        if (fpsPlayer.isAlive == false || isAlive == false)
        {
            return;
        }
        weapon.transform.LookAt(player);
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
        if (isAlive == false)
        {
            return;
        }
        if (health <= 0)
        {
            anim.SetTrigger("Death");
            isAlive = false;
            GetComponent<CharacterController>().enabled = false;
            agent.SetDestination(transform.position);
            return;
        }
        if (currentState == EnemyState.patrol)
        {
            GunShotAlert();
        }
        if (currentState == EnemyState.alert)
        {
            var lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 80);
            playerLastPosition = player.position;
        }
    }

    public void GunShotAlert()
    {
        int chance = Random.Range(0, 101);
        if (chance <= coverChance)
        {
            currentState = EnemyState.cover;
            StartCoroutine(SearchCover());
        }
        else
        {
            currentState = EnemyState.alert;
            anim.SetTrigger("EnterCombat");
        }
    }

    void Patrol()
    {

    }

    FPSPlayer p;
    void Alert()
    {
        GameObject target = SeeActor(transform.position);
        if (target)
        {
            p = target.GetComponent<FPSPlayer>();
            if (p)
            {
                waitingForPatience = false;
                StartShooting();
            }
        }
        else
        {
            shooting = false;
            p = null;
            if (waitingForPatience == false)
            {
                StartCoroutine(Patience());
                waitingForPatience = true;
            }
        }
    }

    void Investigating()
    {
        if (Vector3.Distance(playerLastPosition, transform.position) < 1.5f)
        {
            anim.SetBool("Walking", false);
        }
    }

    IEnumerator Patience()
    {
        yield return new WaitForSeconds(maxPatience);
        if (p == null && currentState != EnemyState.investigating)
        {
            anim.SetBool("Walking", true);
            agent.speed = 1.5f;
            agent.SetDestination(playerLastPosition);
            searchBox.enabled = true;
            currentState = EnemyState.investigating;
        }
    }

    IEnumerator ReactionShoot()
    {
        yield return new WaitForSeconds(reactionTime);
        shooting = true;
    }

    void StartShooting()
    {
        playerLastPosition = player.position;

        var lookPos = p.transform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5);

        if (weapon.cooldown == false)
        {
            if (shooting == false)
            {
                StartCoroutine(ReactionShoot());
            }
            else
            {
                weapon.transform.LookAt(player);
                weapon.Shoot();
            }
        }
    }

    GameObject SeeActor(Vector3 fromWhere)
    {
        Vector3 startPos = fromWhere + Vector3.up;
        Vector3 direction = player.position - startPos;
        Ray scan = new Ray(startPos, direction);
        RaycastHit hit;
        Debug.DrawRay(startPos, direction * 2, Color.blue);
        if (Physics.Raycast(scan, out hit, 30))
        {
            if (hit.collider.CompareTag("Humanoid"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    IEnumerator SearchCover()
    {
        Cover[] availableCovers = FindObjectsOfType<Cover>();
        Cover newCover = availableCovers[Random.Range(0, availableCovers.Length)];
        if (AvailableCover(availableCovers) == false)
        {
            currentState = EnemyState.alert;
            yield break;
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
        agent.speed = 3f;

        while (Vector3.Distance(transform.position, newPos) > 0.75f)
        {
            Debug.Log(Vector3.Distance(transform.position, newPos));
            yield return new WaitForEndOfFrame(); ;
        }
        newCover.taken = false;
        anim.SetTrigger("ExitCover");
        currentState = EnemyState.alert;
    }

    bool IsCoverSafe(Vector3 coverPosition)
    {
        GameObject target = SeeActor(coverPosition);
        if (target)
        {
            FPSPlayer temp = target.GetComponent<FPSPlayer>();
            if (temp != null)
            {
                Debug.Log("UnSafe");
                return false;
            }
        }
        return true;
    }

    bool AvailableCover(Cover[] covers)
    {
        foreach (var c in covers)
        {
            if (c.taken == false && IsCoverSafe(c.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    public void PlayerInSight(Collider other)
    {
        if (currentState == EnemyState.patrol || currentState == EnemyState.investigating)
        {
            FPSPlayer p = other.GetComponent<FPSPlayer>();
            if (p)
            {
                if (SeeActor(transform.position))
                {
                    searchBox.enabled = false;
                    anim.SetBool("Walking", false);
                    currentState = EnemyState.alert;
                    agent.SetDestination(transform.position);
                    GunShotAlert();
                }
            }
        }
    }

}
