using System.Collections.Generic;
using UnityEngine;

public class SanityPillManager : Singleton<SanityPillManager>
{
    [SerializeField] int totalPillsToSpawn = 5;
    [SerializeField] GameObject pillBottle;
    List<Transform> sanityPillPositions = new List<Transform>();
    void Start()
    {
        List<GameObject> gos = new(GameObject.FindGameObjectsWithTag("SanityPillPoint"));
        for (int i = 0; i < totalPillsToSpawn; i++) 
        {
            var f = Random.Range(0, sanityPillPositions.Count);
            sanityPillPositions.Add(gos[f].transform);
            gos.RemoveAt(f);
        }

        foreach (Transform t in sanityPillPositions)
        {
            Instantiate(pillBottle, t);
        }

    }

}
