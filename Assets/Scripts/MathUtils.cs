using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static float Remap(float from_min, float from_max, float to_min, float to_max, float value)
    {
        return to_min + ((value - from_min) / (from_max - from_min)) * (to_max - to_min);
    }
}
