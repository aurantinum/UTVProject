using UnityEngine;
using System.Collections.Generic;

public class GhostPropsManager : MonoBehaviour
{
    public int numberOfJars;
    public int numberOfChairs;
    public int numberOfPotPanBottles;
    public int numberOfPortraits;
    public int numberOfBathroomObj;
    public int numberOfMisc;

    public Transform jarParent;
    public Transform chairParent;
    public Transform ppbParent;
    public Transform portraitsParent;
    public Transform bathroomParent;
    public Transform miscParent;

    public AudioClip ghostPing;

    private List<GameObject> jars;
    private List<GameObject> chairs;
    private List<GameObject> potsPansBottles;
    private List<GameObject> portraits;
    private List<GameObject> bathroomObjs;
    private List<GameObject> misc;

    public List<GameObject> ghostProps;
    private int numberOfGhostProps;

    void Awake()
    {

        // initialize lists
        jars = new List<GameObject>();
        chairs = new List<GameObject>();
        potsPansBottles = new List<GameObject>();
        portraits = new List<GameObject>();
        bathroomObjs = new List<GameObject>();
        misc = new List<GameObject>();
        ghostProps = new List<GameObject>();

        // create lists of each group
        jars = CreateListFrom(jarParent);
        chairs = CreateListFrom(chairParent);
        potsPansBottles = CreateListFrom(ppbParent);
        portraits = CreateListFrom(portraitsParent);
        bathroomObjs = CreateListFrom(bathroomParent);
        misc = CreateListFrom(miscParent);

        // randomly select x props to be ghost props
        AddGhostProps(jars, numberOfJars);
        AddGhostProps(chairs, numberOfChairs);
        AddGhostProps(potsPansBottles, numberOfPotPanBottles);
        AddGhostProps(portraits, numberOfPortraits);
        AddGhostProps(bathroomObjs, numberOfBathroomObj);
        AddGhostProps(misc, numberOfMisc);

        // set the properties of the ghost props
        numberOfGhostProps = numberOfJars + numberOfChairs 
            + numberOfPotPanBottles + numberOfMisc + numberOfPortraits;

        for (int i = 0; i < numberOfGhostProps; i++)
        {
            // set object to ghost layer
            GameObject ghostProp = ghostProps[i];
            ghostProp.layer = LayerMask.NameToLayer("Ghost");
            ghostProp.tag = "GhostProp";

            // stop casting shadows
            MeshRenderer meshRenderer = ghostProp.GetComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // make ping sound
            AudioSource source = ghostProp.AddComponent<AudioSource>();
            source.loop = true;
            source.clip = ghostPing;
            source.volume = 0.2f;
            source.Play();

            // add squish
            SquishAnimation squish = ghostProp.AddComponent<SquishAnimation>();
            squish.squishStrength = 0.08f;
            squish.target = ghostProp.transform;
            squish.enabled = true;

            Debug.Log(i+1 + ": "+ ghostProp.name + " is invisible.");
        }
    }

    // given an empty parent, create a list of GameObjects holding its children
    private List<GameObject> CreateListFrom(Transform parent)
    {
        List<GameObject> props = new List<GameObject>();

        foreach (Transform child in parent)
        {
            props.Add(child.gameObject);
        }

        return props;
    }

    // add a given amount of objects to the ghost group
    private void AddGhostProps(List<GameObject> group, int amt)
    {
        int added = 0;
        while (added < amt) 
        {
            GameObject prop = group[Random.Range(0, group.Count)];
            if (!ghostProps.Contains(prop)) // avoid duplicates
            {
                ghostProps.Add(prop);
                added++;
            }
        }
    }
}
