using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Controller")]
    [SerializeField] FirstPersonController controller;
    [SerializeField] Camera photoCamera;
    [SerializeField] PhotoManager photoManager;

    [Header("Images")]
    [SerializeField] RenderTexture targetTexture;
    [SerializeField] Image blackout;
    [SerializeField] Image pictureDisplay;
    [SerializeField] Material vignetteMat;
    public Dictionary<GameObject, (Sprite sprite, bool hasGhost, bool hasProp)> pictures;
    public (Sprite sprite, bool hasGhost, bool hasProp) newPicture;
    GameObject currentProp;

    [Header("Controls")]
    [SerializeField] int shutterTime = 5;

    public UnityEvent OnAnyPictureTaken = new();
    public UnityEvent OnGhostPictureTaken = new();
    public UnityEvent<GameObject> OnGhostPropPictureTaken = new();
    public UnityEvent OnCameraPutAway = new();
    public UnityEvent OnCameraTakenOut = new();

    private GhostAI ghostAI;

    GameObject ghostObject;


    enum CameraMode { Player, Camera }

    [Header("Set Variables")]
    [SerializeField] float holdPictureTime = 5f;

    [Header("Public Booleans")]
    public bool isCameraMode { get; private set; }

    [Header("Private Variables")]
    [SerializeField] bool _doDebugLog;
    bool takingPicture;
    float waitForUpdate;

    private void Awake()
    {
        UpdateCameras(CameraMode.Player);
        pictures = new();
        ghostAI = FindAnyObjectByType<GhostAI>();
        ghostObject = GameObject.FindGameObjectWithTag("Ghost");
    }

    private void Update()
    {
        // Fallback input check
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnSwitch();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            OnCapture();
        }

        // Every five seconds, update what objects are in view 
        // so that we can do vignette stuff
        if (waitForUpdate > 2)
        {
            FindObjectInView();
            waitForUpdate = 0;
        }
        waitForUpdate += Time.deltaTime;
    }

    void FindObjectInView()
    {
        Dictionary<GameObject, int> collisions = new();
        int maxCollisions = 0;

        float marginX = Screen.width / 10;
        float marginY = Screen.height / 10;

        newPicture.hasGhost = false;
        newPicture.hasProp = false;

        for (float x = 0; x < Screen.width; x += marginX)
        {
            for (float y = 0; y < Screen.height; y += marginY)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(x, y, 0));

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject obj = hit.collider.gameObject;

                    // Add this to our post processing effect determination
                    collisions[obj] = collisions.GetValueOrDefault(obj, 0) + 1;

                    if (hit.collider.CompareTag("Ghost")) newPicture.hasGhost = true;
                    if (hit.collider.CompareTag("GhostProp"))
                    {
                        newPicture.hasProp = true;
                        if (collisions[obj] > maxCollisions)
                        {
                            maxCollisions = collisions[obj];
                            currentProp = obj;
                        }
                    }
                }
            }
        }

        if (newPicture.hasGhost)
        {
            float t = Mathf.Lerp(0, 5, collisions[ghostObject])/5f;
            float intensity = Mathf.Lerp(-2.5f, 4, t);
            float factor = Mathf.Pow(2, intensity);
            vignetteMat.color = new Color(vignetteMat.color.r * factor, vignetteMat.color.g * factor, vignetteMat.color.b * factor);
        }

    }

    void UpdateCameras(CameraMode mode)
    {
        isCameraMode = mode == CameraMode.Camera;

        // Turn on camera crosshairs, border, and images
        controller.crosshair = isCameraMode;
        pictureDisplay.transform.parent.gameObject.SetActive(!isCameraMode);

        // Turn on / off zooming
        controller.enableZoom = isCameraMode;

        // Restrict / enable camera movement
        if (isCameraMode)
        {
            OnCameraTakenOut.Invoke();
            controller.mouseSensitivity = 1;
            controller.maxLookAngle = 180f;
            controller.zoomStepTime = 1f;
            controller.zoomFOV = 20f;
            controller.fov = 45f;
            Camera.main.GetUniversalAdditionalCameraData().SetRenderer(1);
        }
        else
        {
            OnCameraPutAway.Invoke();
            controller.mouseSensitivity = 2;
            controller.maxLookAngle = 180f;
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

    private void OnSwitch()
    {
        if (PauseManager.paused) return;
        if (_doDebugLog) Debug.Log("Switching camera");

        if (takingPicture) return;

        CameraMode changeToMode = controller.crosshair ? CameraMode.Player : CameraMode.Camera;
        UpdateCameras(changeToMode);
    }

    private void OnCapture()
    {
        if (_doDebugLog) Debug.Log("Capturing Picture");

        ghostAI.Pause();
        FindObjectInView();

        if (takingPicture || !isCameraMode) return;

        // Set up for capturing
        photoCamera.gameObject.transform.position = Camera.main.transform.position;
        photoCamera.gameObject.transform.rotation = Camera.main.transform.rotation;
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

        // Reset
        RenderTexture.active = currentRT;

        Camera.main.targetTexture = null;

        // Performs the visual action of taking the picture
        StartCoroutine(TakePicture(picture));

        OnAnyPictureTaken.Invoke();
        if (newPicture.hasGhost)
        {
            if (_doDebugLog) Debug.Log("Ghost is here");
            OnGhostPictureTaken.Invoke();
        }

        // Only adds a picture if the prop hasn't been discovered
        else if (newPicture.hasProp && !pictures.ContainsKey(currentProp))
        {
            OnGhostPropPictureTaken.Invoke(currentProp);
            pictures[currentProp] = newPicture;
            newPicture = new();
        }
    }

    IEnumerator TakePicture(Sprite picture)
    {
        // Freeze the camera
        controller.cameraCanMove = false;
        controller.enableZoom = false;
        controller.UpdateStart();

        SoundManager.PlaySound(SoundType.CameraClick);
        // Check win condition
        bool won = photoManager.HasWon();

        if (won) { 
            WinManager.Instance.WinGame(controller, shutterTime);
            yield break; 
        }

        for (int i = 0; i <= shutterTime; i++)
        {
            blackout.color = new Color(0, 0, 0, i / (float)shutterTime);
            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForEndOfFrame();

        for (int i = shutterTime; i >= 0; i--)
        {
            blackout.color = new Color(0, 0, 0, i / (float)shutterTime);
            yield return new WaitForSeconds(.1f);
        }

        blackout.color = new Color(0, 0, 0, 0);

        // Switch back to normal camera but add image on screen
        pictureDisplay.transform.parent.gameObject.SetActive(true);
        pictureDisplay.sprite = picture;

        // Unfreeze the camera
        controller.cameraCanMove = true;
        controller.enableZoom = true;
        controller.UpdateStart();
        ghostAI.Unpause();


        // Turn off camera
        UpdateCameras(CameraMode.Player);

        takingPicture = false;
    }


}
