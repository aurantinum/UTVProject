using UnityEngine;

public enum SoundType
{
   // add all sounds here
    Footstep, // 0
    CameraClick, // 1
    cave_1, // 2
    cave_2, // 3
    cave_3, // 4
    cave_4, // 5
    cave_5, // 6
    cave_6, // 7
    cave_7, // 8
    cave_8, // 9
    cave_9, // 10
    cave_10, // 11
    cave_11, // 12
    cave_12, // 13
    cave_13, // 14
    cave_14, // 15
    Shh,// 16
    GameLost // 17

}

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip [] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }

    public static void PlaySound(SoundType sound, float volume = .2f)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
