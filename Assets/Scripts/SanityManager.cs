using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SanityManager : Singleton<SanityManager>
{
    #region instance vars and get/sets
    public float StartingSanity {  get; private set; }
    private float _sanity = 360;
    public float Sanity { 
        get { return _sanity; }
        set
        {
            if (_sanity > value) 
                OnSanityLost.Invoke();
            _sanity = value;
        }
    }
    private float _sanityDecayAmount = 1f;
    public float SanityDecayAmount { 
        get { return _sanityDecayAmount; } 
    }
    private float _sanityDecayTime = 1f;
    public float SanityDecayTime {
        get { return _sanityDecayTime; }
        set { _sanityDecayTime = value; } 
    }
    public UnityEvent OnSanityLost = new();

    private bool doDecay = false;
    #endregion
    
    private void Start()
    {
        StartingSanity = _sanity;
        StartDecay();
    }

    public void StartDecay()
    {
        if(!doDecay)
        {
            doDecay = true;
            StartCoroutine("SanityDecay");
        }
    }

    public void StopDecay()
    {
        doDecay = false;
    }


    public IEnumerator SanityDecay()
    {
        while (doDecay)
        {
            yield return new WaitForSeconds(SanityDecayTime);

            Sanity -= SanityDecayAmount;
        }
    }
}
