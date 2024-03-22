using UnityEngine;
using System.Collections.Generic;



[System.Serializable]
public enum BehaviorType
{
    Animation,
    Overlay,
    Selection
}

public abstract class PointBehavior : MonoBehaviour
{
    [SerializeField] protected BehaviorType type;
    public BehaviorType Type { get { return type; } }

    // TODO: Consider if returning only Vector3 is the suitable signature?
    // Maybe void or List<Vector3> is appropriate for overlays, selectors, etc.

    // Calculated on every update
    public abstract Vector3 UpdateBehavior(List<Vector3> InPoints, int ListIndex = -1);
    // Calculated on first update
    public virtual void InitializeBehavior(List<Vector3> InPoints, int ListIndex = -1)
    {
        return;
    }
}
