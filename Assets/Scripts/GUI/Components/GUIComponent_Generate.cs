using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIComponent_Generate : GUIComponent
{
    [SerializeField] private GUIIncrementSliderInput controllerPointCount;
    [SerializeField] private TMP_Dropdown controllerGenerationMethod;

    public float PointCount { get { return controllerPointCount.SliderValue; } }
    public GenerationMethod GenerateMethod { get { return GenerationMethod.Random; } }
}
