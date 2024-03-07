using UnityEngine;

public class PointBehavior_AnimationJitter : PointBehavior_Animate
{
    public float JitterSpeed { get { return controllerJitterSpeed.IncrementalSlider.SliderValue; } }
    public bool NormalizeJitter { get { return controllerNormalizeJitter.Toggle.isOn; } }
    public bool RelativeSpeed { get { return controllerRelativeSpeed.Toggle.isOn; } }
    [SerializeField] private GUIOption_IncrementalSlider2 controllerJitterSpeed;
    public GUIOption_IncrementalSlider2 ControllerJitterSpeed { set { controllerJitterSpeed = value; } }
    [SerializeField] private GUIOption_Toggle2 controllerNormalizeJitter;
    public GUIOption_Toggle2 ControllerNormalizeJitter { set { controllerNormalizeJitter = value; } }
    [SerializeField] private GUIOption_Toggle2 controllerRelativeSpeed;
    public GUIOption_Toggle2 ControllerRelativeSpeed { set { controllerRelativeSpeed = value; } }



    // TODO: When setting relativeSpeed on: clamp slider between 0 and 1. Off: between 0 and length of shortest axis
    // TODO: When setting relativeSpeed, set delegate instead of constantly if-checking
    // TODO: Cache jitterSpeed on value changed only, don't read on every update
    // TODO: Cache normalizeJitter on value changed only, don't read on every update
    public override Vector2 UpdateBehavior(Vector2 inVector)
    {
        float speed = JitterSpeed;
        if (RelativeSpeed)
        {
            // TODO: Optimize cast?
            Vector2 halfBounds = (Vector3)pointManager.Bounds / 2f;
            float relativeMultiplier = Mathf.Min(halfBounds.x, halfBounds.y) * 100f;
            speed *= relativeMultiplier;
        }
        Vector2 direction = new Vector2(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f));
        if (NormalizeJitter)
            direction = Vector2.ClampMagnitude(direction, 1f);

        return direction * speed;
    }

    public override Vector3 UpdateBehavior(Vector3 inVector)
    {
        float speed = JitterSpeed;
        if (RelativeSpeed)
        {
            Vector3 halfBounds = (Vector3)pointManager.Bounds / 2f;
            float relativeMultiplier = Mathf.Min(halfBounds.x, Mathf.Min(halfBounds.y, halfBounds.z)) * 100f;
            speed *= relativeMultiplier;
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
