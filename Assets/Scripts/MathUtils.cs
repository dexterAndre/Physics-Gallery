using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float Remap(float value, float from_min, float from_max, float to_min, float to_max)
    {
        return to_min + ((value - from_min) / (from_max - from_min)) * (to_max - to_min);
    }

    // TODO: Make centerpoint-agnostic
    public static float Wrap(float value, float min, float max)
    {
        if (value < min)
            return value % min;
        else if (value > max)
            return value % max;

        return value;
    }

    public static Vector2 Wrap(Vector2 value, Vector2 min, Vector2 max)
    {
        Vector2 interval = new Vector2(max.x - min.x, max.y - min.y);
        if (value.x < min.x)
            value.x += interval.x;
        else if (value.x > max.x)
            value.x -= interval.x;
        if (value.y < min.y)
            value.y += interval.y;
        else if (value.y > max.y)
            value.y -= interval.y;

        return value;
    }

    public static Vector2 ClosestPointToLine(Vector2 point, Vector2 linePoint, Vector2 lineDirection)
    {
        Vector2 linePointToPoint = point - linePoint;
        return linePoint + (Vector2.Dot(linePointToPoint, lineDirection) / lineDirection.sqrMagnitude) * lineDirection;
    }
}
