using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LightswitchManager : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Light light;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent turnOnEvent;
    [SerializeField] private UnityEvent turnOffEvent;

    [Header("Settings")]
    [SerializeField] private bool startOn;

    [Header("Debugging")]
    [SerializeField] private bool testToggle;
    [SerializeField] private bool testGhost;

    // Local Variable
    private bool isTurnedOn = false;
    private Coroutine ghostRoutine = null;

    private void Awake()
    {
        if (startOn)
        {
            ChangeColors(Color.green);
        }
    }


    private void Update()
    {
        if (testToggle)
        {
            ToggleLight(true);
            testToggle = false;
        }
        if (testGhost)
        {
            GhostInteract();
            testGhost = false;
        }
    }

    public void GhostInteract()
    {
        // Ghost fucks with it
        if (ghostRoutine != null) return; // dont fuck with the switch if already fucking :3
        ghostRoutine = StartCoroutine(GhostTrickeryRoutine());
    }

    public void LookedAt()
    {
        
    }

    public void PlayerInteract()
    {
        // Set them back to normal state
        if (ghostRoutine != null)
        {
            StopCoroutine(ghostRoutine);
            ghostRoutine = null;
        }
        ToggleLight(true);
    }

    private IEnumerator GhostTrickeryRoutine()
    {
        while (true)
        {
            ToggleLight(true);
            yield return new WaitForSeconds(Random.Range(0.05f,0.9f));
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void ToggleLight(bool doPlaySound)
    {
        if (isTurnedOn)
        {
            // was turned on, toggle off
            ChangeColors(Color.red);
            turnOffEvent.Invoke();
            isTurnedOn = false;
        }
        else
        {
            // It was off, toggle it on
            ChangeColors(Color.green);
            turnOnEvent.Invoke();
            isTurnedOn = true;
        }
        if (doPlaySound) audioSource.Play();
    }

    private void ChangeColors(Color color)
    {
        light.color = color;
        meshRenderer.material.color = color;
    }
}
