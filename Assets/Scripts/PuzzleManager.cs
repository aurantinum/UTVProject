using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : Singleton<PuzzleManager>
{
    List<GameObject> ghostProps;
    public UnityEvent<GameObject> OnGhostPropPictureTaken;
    void Start()
    {
        var gos = GameObject.FindGameObjectsWithTag("GhostProp");
        foreach (var g in gos)
        {
            ghostProps.Add(g);
        }
        OnGhostPropPictureTaken.AddListener(Found);
    }

    void Found(GameObject go)
    {
        ghostProps.Remove(go);
        go.GetComponent<Animator>()/*play animation after we return to normal view*/;
    }
   
}
