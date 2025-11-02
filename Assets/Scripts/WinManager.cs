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

    [SerializeField] int distance = 50;


    public void WinGame(FirstPersonController controller, int shutterTime)
    {
        winScreen.SetActive(true);
        StartCoroutine(Polterzoom(winScreen.GetComponentInChildren<Animator>().gameObject, controller, shutterTime));

        // play scary sound
    }

    public void LoseGame()
    {
        loseScreen.SetActive(true);
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
        while (source.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        ReloadLevel();
    }

    IEnumerator Polterzoom(GameObject ghost, FirstPersonController controller, int shutterTime)
    {
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


            blackout.color = new Color(0, 0, 0, i / (float)shutterTime);
            yield return new WaitForSeconds(0.1f);
        }

        blackout.color = new Color(0, 0, 0, 1);
        RemoveUI(controller);

        ghost.transform.localPosition -= (distance / 2) * Vector3.forward;

        yield return new WaitForEndOfFrame();

        blackout.color = new Color(0, 0, 0, 0);


        for (int i = 0; i < (shutterTime/5f); i++)
        {
            ghost.transform.localPosition -= (distance / (shutterTime/5f)) * 0.25f * Vector3.forward;
            yield return new WaitForSeconds(0.1f);
        }

        textColor.a = 1; winText.color = textColor;
        blackout.color = new Color(0, 0, 0, 1);

    }

}
