using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointBehavior_AnimationJitter : PointBehavior_Animate
{
    [SerializeField] private float jitterSpeed = 0.1f;
    public float JitterSpeed { get { return jitterSpeed; } set { jitterSpeed = value; } }
    [SerializeField] private bool normalizeJitter = false;
    public bool NormalizeJitter { get { return normalizeJitter; } set { normalizeJitter = value; } }
    [SerializeField] private bool relativeSpeed = false;
    public bool RelativeSpeed { get { return relativeSpeed; } set {  relativeSpeed = value; } }
    [SerializeField] private GUIIncrementSliderInput controllerJitterSpeed;
    [SerializeField] private Toggle controllerNormalizeJitter;
    [SerializeField] private Toggle controllerRelativeSpeed;

    public override Vector3 UpdateBehavior(Vector3 inVector)
    {
        // TODO: Cache jitterSpeed on value changed only, don't read on every update
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
        // TODO: cache normalizeJitter on value changed only, don't read on every update
        if (NormalizeJitter)
            direction = Vector3.ClampMagnitude(direction, 1f);

        return direction * speed;
    }
}
