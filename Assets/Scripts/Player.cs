using UnityEngine;

public class Player : Singleton<Player>
{
    public void OnAttacked()
    {

        SanityManager.Instance.Sanity -= (1 / 10) * SanityManager.Instance.StartingSanity;

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
