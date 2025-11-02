using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lightning : MonoBehaviour
{
    public AudioSource[] audioSources;
    public float minTimeBetweenLightning;
    public float maxTimeBetweenLightning;

    private float timeUntilNextLightning = 10.0f;
    private float currentTime = 60.0f;

    private void Start()
    {
        currentTime = 0f;
        timeUntilNextLightning = Random.Range(minTimeBetweenLightning, maxTimeBetweenLightning);
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= timeUntilNextLightning)
        {
            SoundManager.PlaySound(SoundType.cave_2,1f);
        
            currentTime = 0.0f;
            StartCoroutine((LightningFlashes(5)));
            timeUntilNextLightning = Random.Range(minTimeBetweenLightning, maxTimeBetweenLightning);
        }
    }

    private IEnumerator LightningFlashes(int amt)
    {
        Material skybox = RenderSettings.skybox;
            
        for (int i = 0; i < amt; i++)
        {
            skybox.SetFloat("_Exposure", 2.0f);
            yield return new WaitForSeconds(0.2f);
            skybox.SetFloat("_Exposure", 1.0f);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
