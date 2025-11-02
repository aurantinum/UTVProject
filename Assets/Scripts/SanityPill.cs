using UnityEngine;

public class SanityPill : MonoBehaviour, IInteractable
{
    [SerializeField]
    float sanityToRestore = 10;
    public void GhostInteract()
    {
        
    }

    public void PlayerInteract()
    {
        SanityManager.Instance.Sanity += sanityToRestore;
        Destroy(gameObject);
    }
}
