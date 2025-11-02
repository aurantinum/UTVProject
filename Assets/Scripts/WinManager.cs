using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : Singleton<WinManager>
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] Image blackout;
    [SerializeField] GameObject sanity;

    PauseManager pauseManager;
    CameraManager cameraManager;

    [SerializeField] int distance = 50;
    [SerializeField] bool win; // DEBUG

    private void Start()
    {
        pauseManager = FindAnyObjectByType<PauseManager>();
        cameraManager = FindAnyObjectByType<CameraManager>();
    }


    public void WinGame(FirstPersonController controller, int shutterTime)
    {
        winScreen.SetActive(true);
        sanity.SetActive(false);
        pauseManager.gameObject.SetActive(false);
        StartCoroutine(Polterzoom(winScreen.GetComponentInChildren<Animator>().gameObject, controller, shutterTime));

        // play scary sound
    }

    public void LoseGame()
    {
        loseScreen.SetActive(true);
        sanity.SetActive(false);
        pauseManager.gameObject.SetActive(false);

        // PLEASE REPLACE NULL WITH AUDIO SOURCE
        StartCoroutine(WaitForCrack(null));
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(1);
    }

    void RemoveUI(FirstPersonController controller)
    {
        // Turn on camera crosshairs, border, and images
        bool isCameraMode = false;
        controller.crosshair = isCameraMode;

        // Turn on / off zooming
        controller.enableZoom = isCameraMode;

        // Restrict / enable camera movement
        if (isCameraMode)
        {
            controller.mouseSensitivity = 1;
            controller.maxLookAngle = 10f;
            controller.zoomStepTime = 1f;
            controller.zoomFOV = 20f;
            controller.fov = 45f;
            Camera.main.GetUniversalAdditionalCameraData().SetRenderer(1);
        }
        else
        {
            controller.mouseSensitivity = 2;
            controller.maxLookAngle = 50f;
            controller.fov = 80f;
            Camera.main.GetUniversalAdditionalCameraData().SetRenderer(0);
        }

        // Restrict / enable player movement
        controller.playerCanMove = !isCameraMode;
        controller.enableCrouch = !isCameraMode;
        controller.enableJump = !isCameraMode;
        controller.isWalking = false; // stop head bob

        controller.UpdateStart();
    }

    IEnumerator WaitForCrack(AudioSource source)
    {
        if (source != null)
        {
            while (source.isPlaying)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator Polterzoom(GameObject ghost, FirstPersonController controller, int shutterTime)
    {
        Image winBlack = winScreen.GetComponent<Image>();
        // Define components
        TMP_Text winText = winScreen.GetComponentInChildren<TMP_Text>();

        ghost.SetActive(true);

        // Define colors
        Color textColor = winText.color;

        textColor.a = 0;
        winText.color = textColor;

        // Update position
        ghost.transform.localPosition += Vector3.forward * distance;

        for (int i = 0; i < shutterTime; i++)
        {
            ghost.transform.localPosition -= (distance / shutterTime) * 0.25f * Vector3.forward;

            winBlack.color = new Color(0, 0, 0, i / (float)shutterTime);
            yield return new WaitForSeconds(0.1f);
        }

        winBlack.color = new Color(0, 0, 0, 1);
        RemoveUI(controller);

        ghost.transform.localPosition -= (distance / 2) * Vector3.forward;

        yield return new WaitForEndOfFrame();

        winBlack.color = new Color(0, 0, 0, 0);


        for (int i = 0; i < (shutterTime); i++)
        {
            ghost.transform.localPosition -= (distance / (shutterTime)) * 0.25f * Vector3.forward;
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(1);
        ghost.SetActive(false); 
        winBlack.color = new Color(0, 0, 0, 1); 
        textColor.a = 1; winText.color = textColor;

    }

}
