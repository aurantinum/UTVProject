using System.Collections;
using UnityEngine;

public class SquishAnimation : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Squish Settings")]
    [SerializeField, Range(0,1)] private float squishStrength = 0.17f;
    [SerializeField] private float squishDuration = 0.17f;

    [Header("Idle Settings")]
    [SerializeField] private bool doIdleSquish = true;
    [SerializeField] private float timeToSquish = 8f;


    [Header("Optional")]
    [SerializeField] private ParticleSystem particleSystem;

    [Header("Debugging")]
    [SerializeField] private bool playSquish;

    // Local Variables
    private Coroutine squishRoutine;
    private Vector3 originalScale;
    private float timePassed;

    private void Start()
    {
        originalScale = target.localScale;
        
    }

    private void Update()
    {
        if (playSquish)
        {
            PlaySquish();
            playSquish = false;
        }

        timePassed += Time.deltaTime;
        if (timePassed >= timeToSquish)
        {
            PlaySquish();
            timePassed = 0;
        }
    }


    /// <summary>
    /// Will play the squish animation on the object
    /// </summary>
    public void PlaySquish()
    {
        if (squishRoutine == null)
        {
            squishRoutine = StartCoroutine(SquishCoroutine());
        }
    }

    private IEnumerator SquishCoroutine()
    {
        // Play the particle system
        if (particleSystem) particleSystem.Play();

        // Animate
        float halfDuration = squishDuration / 2f;
        Vector3 squishedScale = new Vector3(
            originalScale.x + squishStrength,
            originalScale.y - squishStrength,
            originalScale.z + squishStrength
        );

        // Squish down
        float t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float normalized = t / halfDuration;
            transform.localScale = Vector3.Lerp(originalScale, squishedScale, normalized);
            yield return null;
        }

        // Unsquish back to normal
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float normalized = t / halfDuration;
            transform.localScale = Vector3.Lerp(squishedScale, originalScale, normalized);
            yield return null;
        }

        // Finished
        transform.localScale = originalScale;
        squishRoutine = null;
    }
}
