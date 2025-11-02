using UnityEngine;

public enum SoundType
{
   // add all sounds here
    Footstep, // 0
    CameraClick, // 1
    amb_house, // 2
    amb_stage1, // 3
    amb_stage2, // 4
    amb_stage3, // 5
    amb_stage4, // 6
    amb_stage5, // 7
    cave_1, // 8
    cave_2, // 9
    cave_3, // 10
    cave_4, // 11
    cave_5, // 12
    cave_6, // 13
    cave_7, // 14
    cave_8, // 15
    cave_9, // 16
    cave_10, // 17
    cave_11, // 18
    cave_12, // 19
    cave_13, // 20
    cave_14 // 21

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
        PlaySound(SoundType.amb_house, 1f);
    }

    public static void PlaySound(SoundType sound, float volume = .2f)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
