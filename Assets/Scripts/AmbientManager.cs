using UnityEngine;

public enum AmbientType
{
   // add all sounds here
   CameraClick, // Assets/Arts/Audio/switch 22
   HitAnvil // Assets/Horror Elements/Anvil_Hit
}

[RequireComponent(typeof(AudioSource))]

public class AmbientManager : MonoBehaviour
{
    [SerializeField] private AudioClip [] soundList;
    private static AmbientManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlayAmbientSound(AmbientType sound, float volume = 1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
