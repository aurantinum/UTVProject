using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    [SerializeField]
    Canvas mainUI;
    [SerializeField]
    Transform tutorialPlacementPoint;
    Dictionary<string, GameObject> tutorialObjects;
    List<string> names;

    bool FCPACalled, FCTOCalled, FIHCalled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        names = new();
        names.Add("Camera Instruction");
        names.Add("Interact Instruction");
        names.Add("Look Instruction");
        names.Add("Move Instruction");
        names.Add("Scrapbook Instruction");
        names.Add("Take Photo Instruction");
        names.Add("Zoom Instruction");

        tutorialObjects = new Dictionary<string, GameObject>();
        foreach (string name in names)
        {
            tutorialObjects[name] = Resources.Load<GameObject>($"Tutorials/{name}");
        }
        StartCoroutine(nameof(ShowMoveAndLookRoutine));

        CameraManager.Instance.OnCameraPutAway.AddListener(FirstCameraPutAway);
        CameraManager.Instance.OnCameraTakenOut.AddListener(FirstCameraTakenOut);
        //FindFirstObjectByType<InteractionManager>().OnInteractableHovered.AddListener(FirstInteractHover);
    }
    void FirstCameraPutAway()
    {
        if (FCPACalled) return;
        StartCoroutine(nameof(ShowScrapbookTutorial));
        FCPACalled = true;
    }
    void FirstCameraTakenOut()
    {
        if (FCTOCalled) return;
        StartCoroutine(nameof(ShowCameraTutorial));
        FCTOCalled = true;
    }
    void FirstInteractHover(GameObject go)
    {
        if(FIHCalled) return;
        StartCoroutine(nameof(ShowInteractTutorial));
        FIHCalled = true;
    }
    void RemoveTPPChildren()
    {
        for(int i = tutorialPlacementPoint.childCount - 1; i >= 0; i--)
        {
            Destroy(tutorialPlacementPoint.GetChild(i).gameObject);
        }
    }

    IEnumerator ShowMoveAndLookRoutine()
    {
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[3]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[1]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[0]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
    }

    IEnumerator ShowInteractTutorial()
    {
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[1]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
    }

    IEnumerator ShowCameraTutorial()
    {
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[6]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[5]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
    }

    IEnumerator ShowScrapbookTutorial()
    {
        RemoveTPPChildren();
        yield return null;
        Instantiate(tutorialObjects[names[4]], tutorialPlacementPoint);
        yield return new WaitForSeconds(5);
        RemoveTPPChildren();
    }
}
