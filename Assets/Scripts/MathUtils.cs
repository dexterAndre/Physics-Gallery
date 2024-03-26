using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float TAU = Mathf.PI * 2f;

    public static float Remap(float value, float from_min, float from_max, float to_min, float to_max)
    {
        return to_min + ((value - from_min) / (from_max - from_min)) * (to_max - to_min);
    }

    public static int Wrap(int value, int min, int max)
    {
        int modulo = value % (max - min);

        if (modulo >= 0)
            return min + value % (max - min);
        else
            return max + value % (max - min);
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

    public static Vector2 Wrap(Vector2 InPoint, Vector2 InMin, Vector2 InMax)
    {
        Vector2 interval = InMax - InMin;
        if (InPoint.x < InMin.x)
            InPoint.x += interval.x;
        else if (InPoint.x > InMax.x)
            InPoint.x -= interval.x;
        if (InPoint.y < InMin.y)
            InPoint.y += interval.y;
        else if (InPoint.y > InMax.y)
            InPoint.y -= interval.y;

        return InPoint;
    }

    public static Vector3 Wrap(Vector3 InPoint, Vector3 InMin, Vector3 InMax)
    {
        Vector3 interval = InMax = InMin;
        if (InPoint.x < InMin.x)
            InPoint.x += interval.x;
        else if (InPoint.x > InMax.x)
            InPoint.x -= interval.x;
        if (InPoint.y < InMin.y)
            InPoint.y += interval.y;
        else if (InPoint.y > InMax.y)
            InPoint.y -= interval.y;
        if (InPoint.z < InMin.z)
            InPoint.z += interval.z;
        else if (InPoint.z > InMax.z)
            InPoint.z -= interval.z;

        return InPoint;
    }

    public static Vector2 ClosestPointToLine(Vector2 point, Vector2 linePoint, Vector2 lineDirection)
    {
        Vector2 linePointToPoint = point - linePoint;
        return linePoint + (Vector2.Dot(linePointToPoint, lineDirection) / lineDirection.sqrMagnitude) * lineDirection;
    }

    public static bool IsInside_AABB_2D(Vector3 InPoint, Vector2 InBoundsHalf, ref Vector3 OutNearestBoundary)
    {
        // TODO: Consider catching the case where you need to wrap onto a corner
        if (InPoint.x < -InBoundsHalf.x)
        {
            OutNearestBoundary = ClosestPointToLine(InPoint, -InBoundsHalf, Vector2.up);
            return false;
        }
        else if (InPoint.x > InBoundsHalf.x)
        {
            OutNearestBoundary = ClosestPointToLine(InPoint, InBoundsHalf, Vector2.down);
            return false;
        }
        else if (InPoint.y < -InBoundsHalf.y)
        {
            OutNearestBoundary = ClosestPointToLine(InPoint, -InBoundsHalf, Vector2.right);
            return false;
        }
        else if (InPoint.y > InBoundsHalf.y)
        {
            OutNearestBoundary = ClosestPointToLine(InPoint, InBoundsHalf, Vector2.left);
            return false;
        }

        return true;
    }
    public static bool IsInside_AABB_3D(Vector3 InPoint, Vector3 InBoundsHalf, ref Vector3 OutNearestBoundary)
    {
        // TODO: Consider catching the case where you need to wrap onto a corner
        if (InPoint.x < -InBoundsHalf.x)
        {
            OutNearestBoundary = new Vector3(-InBoundsHalf.x, InPoint.y, InPoint.z);
            return false;
        }
        else if (InPoint.x > InBoundsHalf.x)
        {
            OutNearestBoundary = new Vector3(InBoundsHalf.x, InPoint.y, InPoint.z);
            return false;
        }
        else if (InPoint.y < -InBoundsHalf.y)
        {
            OutNearestBoundary = new Vector3(InPoint.x, -InBoundsHalf.y, InPoint.z);
            return false;
        }
        else if (InPoint.y > InBoundsHalf.y)
        {
            OutNearestBoundary = new Vector3(InPoint.x, InBoundsHalf.y, InPoint.z);
            return false;
        }
        else if (InPoint.z < -InBoundsHalf.z)
        {
            OutNearestBoundary = new Vector3(InPoint.x, InPoint.y, -InBoundsHalf.z);
            return false;
        }
        else if (InPoint.z > InBoundsHalf.z)
        {
            OutNearestBoundary = new Vector3(InPoint.x, InPoint.y, InBoundsHalf.z);
            return false;
        }

        return true;
    }
    public static bool IsInside_Sphere(Vector3 InPoint, float InRadius, ref Vector3 OutNearestBoundary)
    {
        if (InPoint.sqrMagnitude > (InRadius * InRadius))
        {
            OutNearestBoundary = InPoint.normalized * InRadius;
            return false;
        }

        return true;
    }
}
