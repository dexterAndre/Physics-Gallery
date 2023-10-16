using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehavior_AnimationVectorField : PointBehavior_Animate
{
    // TODO: Actually implement a vector field function and parser
    [SerializeField] private Vector3 direction;

    public override Vector3 UpdateBehavior(Vector3 inVector)
    {
        return direction;
    }
}
