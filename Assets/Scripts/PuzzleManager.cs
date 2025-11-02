using System.Collections.Generic;
using UnityEngine;


public class PuzzleManager : Singleton<PuzzleManager>
{
    public List<GameObject> ghostProps { get; private set; }
    
    void Start()
    {
        ghostProps = FindFirstObjectByType<GhostPropsManager>().ghostProps;
        CameraManager.Instance.OnGhostPropPictureTaken.AddListener(Found);
    }

    void Found(GameObject go)
    {
        ghostProps.Remove(go);
        go.layer = LayerMask.NameToLayer("Default");
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        go.GetComponent<Animator>()/*play animation after we return to normal view*/;
    }
   
}
