using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviorAnimation_VectorField : PointBehaviorAnimation2
{
    // TODO: Actually implement a vector field function and parser
    [SerializeField] private Vector3 direction;

    public override Vector2 UpdateBehavior()
    {
        return direction;
    }
}
