using UnityEngine;

public class Player : Singleton<Player>
{
    public void OnAttacked()
    {
        if(SanityManager.Instance.Sanity < (1/10) * SanityManager.Instance.StartingSanity)
        {
            //lose game
        }
        else
        {
            //disable camera for short time 


            SanityManager.Instance.Sanity -= (1/15) * SanityManager.Instance.StartingSanity;

        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
