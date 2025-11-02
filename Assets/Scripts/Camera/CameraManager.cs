using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] FirstPersonController controller;

    [Header("Images")]
    [SerializeField] RenderTexture targetTexture;
    [SerializeField] Image blackout;
    [SerializeField] Image pictureDisplay;
    public List<(Sprite sprite, bool hasGhost)> pictures;
    (Sprite sprite, bool hasGhost) newPicture;


    enum CameraMode { Player, Camera }

    [Header("Set Variables")]
    [SerializeField] float holdPictureTime = 5f;

    [Header("Public Booleans")]
    public bool isCameraMode { get; private set; }

    [Header("Private Booleans")]
    bool _doDebugLog;
    bool takingPicture;
    bool prevIsZoomed;

    private void Awake()
    {
        UpdateCameras(CameraMode.Player);
        pictures = new();
    }

    private void Update()
    {
        if (isCameraMode) FindObjectInView();
        prevIsZoomed = controller.isZoomed;
    }

    void FindObjectInView()
    {
        Dictionary<GameObject, int> collisions = new();

        float marginX = 5;
        float marginY = 5;

        newPicture.hasGhost = false;

        for (float x=0; x < Screen.width; x += marginX)
        {
            for (float y=0; y < Screen.height; y += marginY)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject obj = hit.collider.gameObject;

                    // Add this to our post processing effect determination
                    collisions[obj] = collisions.GetValueOrDefault(obj, 0) + 1;

                    if (hit.collider.CompareTag("Ghost")) newPicture.hasGhost = true;
                }
            }
        }
        

        controller.crosshairObject.color = Color.black;

    }

    void UpdateCameras(CameraMode mode)
    {
        isCameraMode = mode == CameraMode.Camera;

        // Turn on camera crosshairs and border
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
        }
        else
        {
            controller.mouseSensitivity = 2;
            controller.maxLookAngle = 50f;
            controller.fov = 80f;
        }

        // Restrict / enable player movement
        controller.playerCanMove = !isCameraMode;
        controller.enableCrouch = !isCameraMode;
        controller.enableJump = !isCameraMode;
        controller.isWalking = false; // stop head bob

        controller.UpdateStart();
    }

    private void OnSwitch()
    {
        if (_doDebugLog) Debug.Log("Switching camera");

        if (takingPicture) return;

        CameraMode changeToMode = controller.crosshair ? CameraMode.Player : CameraMode.Camera;
        UpdateCameras(changeToMode);
    }

    private void OnCapture()
    {
        if (_doDebugLog) Debug.Log("Capturing Picture");

        if (takingPicture || !isCameraMode) return;

        // Set up for capturing
        takingPicture = true;
        Camera.main.targetTexture = targetTexture;
        controller.crosshairObject.gameObject.SetActive(false);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Camera.main.targetTexture;

        // Capture image
        Camera.main.Render();

        Texture2D image = new(Camera.main.targetTexture.width, Camera.main.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.main.targetTexture.width, Camera.main.targetTexture.height), 0, 0);
        image.Apply();

        Sprite picture = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        newPicture.sprite = picture;
        pictures.Add(newPicture);
        newPicture = new();

        // Reset
        RenderTexture.active = currentRT;

        Camera.main.targetTexture = null;

        // Performs the visual action of taking the picture
        StartCoroutine(TakePicture(picture));
    }

    IEnumerator TakePicture(Sprite picture)
    {
        // Freeze the camera
        controller.cameraCanMove = false;
        controller.enableZoom = false;
        controller.UpdateStart();

        for (float i = 0; i <= 1; i += 0.1f)
        {
            if (i < 0.5f) blackout.color = new Color(0, 0, 0, i * 2);
            else blackout.color = new Color(0, 0, 0, (1 - i) * 2);
            yield return new WaitForEndOfFrame();
        }


        // Switch back to normal camera but add image on screen
        int randRot = Random.Range(-10, 10);
        pictureDisplay.transform.parent.localRotation = Quaternion.identity;
        pictureDisplay.transform.parent.Rotate(new Vector3(0, 0, randRot));
        pictureDisplay.transform.parent.gameObject.SetActive(true);
        pictureDisplay.sprite = picture;
        yield return new WaitForSeconds(holdPictureTime);

        // remove image on screen, reset crosshair, etc
        pictureDisplay.transform.parent.gameObject.SetActive(false);
        controller.crosshairObject.gameObject.SetActive(true);

        // Unfreeze the camera
        controller.cameraCanMove = true;
        controller.enableZoom = true;
        controller.UpdateStart();

        takingPicture = false;
    }
}
