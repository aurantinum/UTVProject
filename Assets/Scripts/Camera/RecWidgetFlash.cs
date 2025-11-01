using UnityEngine;

public class RecWidgetFlash : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float flashDuration;
    [SerializeField] private GameObject recCircle;

    // Local Variables
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= flashDuration)
        {
            recCircle.SetActive(!recCircle.activeSelf);
            timer = 0f;
        }
    }
}
