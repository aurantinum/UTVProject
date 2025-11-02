using System.Collections.Generic;
using UnityEngine;


public class PuzzleManager : Singleton<PuzzleManager>
{
    public List<GameObject> ghostProps { get; private set; }
    
    void Start()
    {
        var gos = GameObject.FindGameObjectsWithTag("GhostProp");
        foreach (var g in gos)
        {
            ghostProps.Add(g);
        }
        CameraManager.Instance.OnGhostPropPictureTaken.AddListener(Found);
    }

    void Found(GameObject go)
    {
        ghostProps.Remove(go);
        go.layer = LayerMask.NameToLayer("Default");
        go.GetComponent<Animator>()/*play animation after we return to normal view*/;
    }
   
}
