using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBehaviorAnimation_Jitter2 : PointBehaviorAnimation2
{
    [SerializeField] private float jitterSpeed = 0.1f;
    [SerializeField] private bool normalizeJitter = false;

    public override Vector2 UpdateBehavior()
    {
        float speed = jitterSpeed;
        Vector2 randomDirection = new Vector2(
            Random.Range(-speed, speed),
            Random.Range(-speed, speed));
        if (normalizeJitter)
            randomDirection = Vector2.ClampMagnitude(randomDirection, speed);

        return randomDirection;
    }
}
