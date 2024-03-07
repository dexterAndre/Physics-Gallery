using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehavior_AnimationFlocking : PointBehavior_Animate
{
    // TODO: Actually implement a vector field function and parser
    [SerializeField] private Vector3 direction;

    public override Vector2 UpdateBehavior(Vector2 inVector)
    {
        return direction;
    }

    public override Vector3 UpdateBehavior(Vector3 inVector)
    {
        return direction;
    }
}
