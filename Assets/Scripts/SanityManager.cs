using System.Collections;
using UnityEngine;

public class SanityManager : Singleton<SanityManager>
{
    private float _sanity;
    public float Sanity { get { return _sanity; } }
    private float _sanityDecayAmount;
    public float SanityDecayAmount { get { return _sanityDecayAmount; } }

    private void Start()
    {
        
    }


    public IEnumerator SanityDecay()
    {

        yield return new WaitForSeconds(2.5f);

        _sanity -= 1;
    }
}
