using UnityEngine;
using System.Collections.Generic;



/*
    To do:
    [ ] When setting relativeSpeed on: clamp slider between 0 and 1. (?)
    [ ] When setting relativeSpeed, set delegate instead of constantly if-checking
    [ ] Cache jitterSpeed on value change only, don't read on every update
    [ ] Cache normalizeJitter on value changed only, don't read on every update
*/
public class PointBehavior_AnimationJitter : PointBehavior
{
    public float JitterSpeed { get { return controllerJitterSpeed.IncrementalSlider.SliderValue; } }
    public bool NormalizeJitter { get { return controllerNormalizeJitter.Toggle.isOn; } }
    public bool RelativeSpeed { get { return controllerRelativeSpeed.Toggle.isOn; } }
    [SerializeField] private GUIOption_IncrementalSlider controllerJitterSpeed;
    public GUIOption_IncrementalSlider ControllerJitterSpeed { set { controllerJitterSpeed = value; } }
    [SerializeField] private GUIOption_Toggle controllerNormalizeJitter;
    public GUIOption_Toggle ControllerNormalizeJitter { set { controllerNormalizeJitter = value; } }
    [SerializeField] private GUIOption_Toggle controllerRelativeSpeed;
    public GUIOption_Toggle ControllerRelativeSpeed { set { controllerRelativeSpeed = value; } }
    [SerializeField] Manager_PointSet managerPointSet;



    private void OnEnable()
    {
        if (managerPointSet == null)
        {
            managerPointSet = GameObject.Find("Points Parent").GetComponent<Manager_PointSet>();
            if (managerPointSet == null)
            {
                Debug.LogWarning("Could not find Manager_PointSet. Cannot use Relative Speed.");
            }
        }
    }

    public override Vector3 UpdateBehavior(List<Vector3> InPoints, int ListIndex = -1)
    {
        float speed = JitterSpeed;
        if (RelativeSpeed)
        {
            if (managerPointSet)
            {
                Vector3 halfBounds = (Vector3)managerPointSet.Bounds / 2f;
                float relativeMultiplier = Mathf.Min(halfBounds.x, Mathf.Min(halfBounds.y, halfBounds.z));
                speed *= relativeMultiplier;
            }
        }
        Vector3 direction = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f));
        if (NormalizeJitter)
            direction = Vector3.ClampMagnitude(direction, 1f);

        return direction * speed;
    }
}
