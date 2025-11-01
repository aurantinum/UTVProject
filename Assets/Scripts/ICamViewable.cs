using UnityEngine;

public interface ICamViewable
{
    public bool IsGhostFrozen { get; set; }

    public void OnViewed();
}
