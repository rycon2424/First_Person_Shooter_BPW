using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public State currentState;
    public GameObject radar;
    [Header("Stats")]
    [Range(0, 100)] public int coverChance;
    public float reactionTime;
    public float maxPatience;
    public Vector3 viewDistancePatrol = new Vector3(5, 1, 1);
    public Vector3 viewDistanceAlert = new Vector3(7, 2, 2);
    [Space]
    public Weapon weapon;

    [HideInInspector] public StateMachine statemachine;
    [HideInInspector] public Vector3 playerLastPosition;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Transform player;
    [HideInInspector] public FPSPlayer fpsPlayer;
    [HideInInspector] public bool shooting;
    [HideInInspector] public bool waitingForPatience;
    [HideInInspector] public MeshCollider searchBox;
    [HideInInspector] public FPSPlayer tempPlayer;
    [HideInInspector] public Vector3 eyeOffset;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        fpsPlayer = FindObjectOfType<FPSPlayer>();
        player = fpsPlayer.transform;
        searchBox = GetComponentInChildren<MeshCollider>();
        eyeOffset = new Vector3(0, 0.65f, 0);
        SetupStateMachine();
    }

    void SetupStateMachine()
    {
        statemachine = new StateMachine();
        InvestigationState it = new InvestigationState();
        CoverState cs = new CoverState();
        PatrolState ps = new PatrolState();
        AlertState at = new AlertState();
        DeathState ds = new DeathState();

        statemachine.allStates.Add(it);
        statemachine.allStates.Add(cs);
        statemachine.allStates.Add(ps);
        statemachine.allStates.Add(at);
        statemachine.allStates.Add(ds);
        statemachine.GoToState(this, "PatrolState");
    }

    void Update()
    {
        if (fpsPlayer.isAlive == false || isAlive == false)
        {
            return;
        }
        currentState.StateUpdate(this);
        weapon.transform.LookAt(player);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        if (statemachine.IsInState("DeathState"))
        {
            return;
        }
        if (health <= 0)
        {
            statemachine.GoToState(this ,"DeathState");
            return;
        }
        if (statemachine.IsInState("PatrolState"))
        {
            GunShotAlert();
        }
    }

    public void GunShotAlert()
    {
        int chance = Random.Range(0, 101);
        waitingForPatience = false;
        if (chance <= coverChance && statemachine.IsInState("PatrolState"))
        {
            StartCoroutine(SearchCover());
            return;
        }
        statemachine.GoToState(this, "AlertState");
    }
    
    public void StartRoutine(string Cname)
    {
        StartCoroutine(Cname);
    }

    IEnumerator Patience()
    {
        yield return new WaitForSeconds(maxPatience);
        if (tempPlayer == null && !statemachine.IsInState("InvestigationState"))
        {
            statemachine.GoToState(this, "InvestigationState");
        }
    }

    IEnumerator ReactionShoot()
    {
        yield return new WaitForSeconds(reactionTime);
        shooting = true;
    }
    
    IEnumerator SearchCover()
    {
        Cover[] availableCovers = FindObjectsOfType<Cover>();
        if (availableCovers.Length == 0)
        {
            statemachine.GoToState(this, "AlertState");
            yield break;
        }
        Cover newCover = availableCovers[Random.Range(0, availableCovers.Length)];
        if (AvailableCover(availableCovers) == false)
        {
            statemachine.GoToState(this, "AlertState");
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

        statemachine.GoToState(this, "CoverState");

        while (Vector3.Distance(transform.position, newPos) > 0.75f)
        {
            //Debug.Log(Vector3.Distance(transform.position, newPos));
            yield return new WaitForEndOfFrame(); ;
        }
        newCover.taken = false;
        statemachine.GoToState(this, "AlertState");
    }


    public GameObject SeeActor(Vector3 fromWhere)
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
                FPSPlayer t = hit.collider.GetComponent<FPSPlayer>();
                if (t)
                {
                    return hit.collider.gameObject;
                }
            }
        }
        return null;
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
        if (statemachine.IsInState("PatrolState") || statemachine.IsInState("InvestigationState"))
        {
            if (SeeActor(transform.position + eyeOffset))
            {
                statemachine.GoToState(this, "AlertState");
                GunShotAlert();
            }
        }
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
    }

}
