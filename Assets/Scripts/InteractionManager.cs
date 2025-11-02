using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private LayerMask layerMask = ~0;
    [SerializeField] private bool autoInteractEveryFrame = true;

    public IInteractable Current { get; private set; }

    private void Reset()
    {
        if (Camera.main != null) rayOrigin = Camera.main.transform;
        else rayOrigin = transform;
    }

    private void Awake()
    {
        if (rayOrigin == null)
        {
            var cam = Camera.main;
            rayOrigin = cam ? cam.transform : transform;
        }
    }

    private void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.cyan);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log($"we hit = {hit.collider.gameObject.name}");
            if (!hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable)) return;

            Current = interactable;
            if (autoInteractEveryFrame)
                Current.PlayerInteract();

            if (Input.GetKeyDown(KeyCode.E))
            {
                Current.PlayerInteract();
            }

            return;

        }

        Current = null;
    }
}
