using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    public Camera camera;
    private UniversalAdditionalCameraData cameraData;
    public int rendererIndex = 0;

    private void Start()
    {
        cameraData = camera.GetUniversalAdditionalCameraData();
    }

    void Update()
    {
        cameraData.SetRenderer(rendererIndex);
    }
}
