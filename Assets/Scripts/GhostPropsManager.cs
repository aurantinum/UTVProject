using UnityEngine;
using System.Collections.Generic;

public class GhostPropsManager : MonoBehaviour
{
    public int numberOfGhostProps = 8;

    private List<GameObject> props;
    private List<GameObject> ghostProps;

    void Start()
    {
        props = new List<GameObject>();
        ghostProps = new List<GameObject>();

        // create a list of all the props
        foreach (Transform child in transform)
        {
            props.Add(child.gameObject);
        }

        // randomly select x props to be ghost props
        for (int i = 0; i < numberOfGhostProps; i++)
        {
            GameObject prop = props[Random.Range(0, props.Count)];
            if (!ghostProps.Contains(prop)) // avoid duplicates
            {
                ghostProps.Add(prop);
            }

        }

        // set the properties of the ghost props
        for (int i = 0; i < numberOfGhostProps; i++)
        {
            GameObject ghostProp = ghostProps[i];
            ghostProp.layer = LayerMask.NameToLayer("Ghost");
            ghostProp.tag = "GhostProp";
            Debug.Log(ghostProp.name + " is invisible.");
        }
    }
}
