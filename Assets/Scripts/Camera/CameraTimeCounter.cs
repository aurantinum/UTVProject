using TMPro;
using UnityEngine;

public class CameraTimeCounter : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private TextMeshProUGUI text;

    // Local Variables
    private float time = 0f;


    private void Update()
    {
        time += Time.deltaTime;

        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        text.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
