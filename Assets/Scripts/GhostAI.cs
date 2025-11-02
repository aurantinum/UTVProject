using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    public enum GhostState
    {
        WAIT,
        WANDER,
        MEDDLE,
        HUNT,
        FROZEN,
    }

    private GhostState state;
    public GhostState State { get { return state; } }
    private NavMeshAgent agent;
    public float Enrage = 0;
    public Vector3 wanderCenter;
    public float WanderRadius = 1;
    public float TimeFrozen {  get; private set; }
    public float CurrentFreezeLength {  get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wanderCenter = transform.position;
        StartCoroutine(nameof(WaitRoutine), 0.5f);
        FindFirstObjectByType<CameraManager>().OnPictureTaken.AddListener(Freeze);
    }

    //GHOST CRAWL TOWARDS PLAYER

    //WEEPING ANGEL?

    //GHOST CAN KILL OR CAN SHUT OFF CAMERA



    // Update is called once per frame
    void Update()
    {
        
    }


    public void Freeze()
    {
        StopAllCoroutines();
        CurrentFreezeLength = 4;
        gameObject.layer = LayerMask.NameToLayer("FrozenGhost");
        var camViewables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICamViewable>();
        foreach (var camViewable in camViewables) camViewable.IsGhostFrozen = true;
        StartCoroutine(nameof(FreezeRoutine));
    }

    IEnumerator FreezeRoutine()
    {
        state = GhostState.FROZEN;
        TimeFrozen = 0f;
        Enrage += 1;
        var baseC = GetComponent<MeshRenderer>().material.color;
        while (TimeFrozen < CurrentFreezeLength)
        {
            var c = GetComponent<MeshRenderer>().material.color;
            c.a = 1 - TimeFrozen/CurrentFreezeLength;
            GetComponent<MeshRenderer>().material.color = c;
            TimeFrozen += Time.deltaTime;
            transform.position = transform.position;
            transform.rotation = transform.rotation;
            agent.isStopped = true;
            yield return new WaitForEndOfFrame();
        }
        var camViewables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ICamViewable>();
        foreach (var camViewable in camViewables) camViewable.IsGhostFrozen = false;
        GetComponent<MeshRenderer>().material.color = baseC;
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        StartCoroutine(nameof(WaitRoutine), 0);
    }

    IEnumerator MeddleRoutine(float meddleRadius)
    {
        state = GhostState.MEDDLE;
        var ni = FindNearestInteractable(meddleRadius);
        if (ni != null)
        {
            Vector3 diff = ni.position - transform.position;
            float curDistance = diff.sqrMagnitude;
            agent.isStopped = false;
            agent.destination = ni.position;
            while (curDistance > .5f)
            {
                diff = ni.position - transform.position;
                curDistance = diff.sqrMagnitude;
                agent.destination = ni.position;
                yield return null;
            }
            (ni as IInteractable).GhostInteract();
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(nameof(WaitRoutine), .15f);
        }
        else
        {
            StartCoroutine(nameof(WanderRoutine));
        }
    }

    IEnumerator WaitRoutine(float waitTime)
    {
        state = GhostState.WAIT;
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTime);
        if (Random.Range(0, 100) > 50 + Enrage)//lower amount to wander, as enrage increases the chance to meddle does too.
        {
            StartCoroutine(nameof(WanderRoutine));
        }
        else
        {
            if (Random.Range(0, 25) < Enrage)
            {
                StartCoroutine(nameof(HuntRoutine), Enrage);
            }
            else
            {
                StartCoroutine(nameof(MeddleRoutine), 3 + Enrage);
            }
        }
    }
    
    IEnumerator WanderRoutine()
    {
        Vector3 wanderPos = (new Vector3(Random.value, 0, Random.value).normalized * Random.Range(-WanderRadius, WanderRadius)) + 
            PuzzleManager.Instance.ghostProps[Random.Range(0, PuzzleManager.Instance.ghostProps.Count)].transform.position;
        state = GhostState.WANDER;
        agent.isStopped = false;
        agent.destination = wanderPos;
        while (Vector3.Distance(transform.position, agent.destination) > 0.5f)
        {
            yield return null;
        }
        StartCoroutine(nameof(WaitRoutine), 1);
    }

    IEnumerator HuntRoutine(float huntTime)
    {
        state = GhostState.HUNT;
        
        var pt = Player.Instance.transform;
        float timer = 0;
        Vector3 diff = pt.position - transform.position;
        float curDistance = diff.sqrMagnitude;
        agent.isStopped = false;
        agent.destination = pt.position;
        while (timer < huntTime && curDistance > .5f)
        {
            timer += Time.deltaTime;
            diff = pt.position - transform.position;
            curDistance = diff.sqrMagnitude;
            agent.destination = pt.position;
            yield return new WaitForEndOfFrame();
        }
        if(curDistance < .5f)
        {
            Player.Instance.OnAttacked();
        }
        StartCoroutine(nameof(WaitRoutine), 0.25f);

    }


    #region Util Functions
    public Transform FindNearestInteractable(float radius)
    {
        var mbs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        List<MonoBehaviour> interactables = new();
        foreach(var mb in mbs)
        {
            if(mb is IInteractable)
            {
                interactables.Add(mb);
            }
        }
        Transform closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (var interactable in interactables)
        {
            Vector3 diff = interactable.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (distance < radius)
            {
                if (curDistance < distance)
                {
                    closest = interactable.transform;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
    
    
    #endregion
}
