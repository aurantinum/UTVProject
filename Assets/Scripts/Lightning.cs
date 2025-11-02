using UnityEngine;

public class Lightning : MonoBehaviour
{
    public AudioSource[] audioSources;

    private void Start()
    {
        foreach (var source in audioSources)
            source.Play();
    }
}
