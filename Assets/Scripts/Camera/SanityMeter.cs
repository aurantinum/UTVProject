using UnityEngine;
using UnityEngine.UI;

public class SanityMeter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;

    // Singleton
    public static SanityMeter Instance;

    private void Awake()
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;
    }

    /// <summary>
    /// Set the sanity slider of the game
    /// </summary>
    /// <param name="sanity"> a value from 0 to 100 </param>
    public void SetSanity(int sanity)
    {
        slider.value = sanity;
    }

    public void SetMaxSanity(int newMax)
    {
        slider.maxValue = newMax;
    }

    public int GetSanity()
    {
        return (int)slider.value;
    }
}
