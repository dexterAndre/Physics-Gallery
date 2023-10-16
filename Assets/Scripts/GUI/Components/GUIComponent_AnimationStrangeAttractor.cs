using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIComponent_AnimationStrangeAttractor : GUIComponent
{
    [SerializeField] private GUIIncrementSliderInput controllerAttractorSpeed;
    [SerializeField] private GUIController_Dropdown controllerAttractorType;

    public float AttractorSpeed { get { return controllerAttractorSpeed.SliderValue; } }
    public StrangeAttractorType AttractorType { get { return (StrangeAttractorType)controllerAttractorType.Value; } }

    public void UpdateAttractorSpeed()
    {
        ((PointBehavior_AnimationStrangeAttractor)behavior).AttractorSpeed = AttractorSpeed;
    }
    public void UpdateAttractorType()
    {
        ((PointBehavior_AnimationStrangeAttractor)behavior).SetType(AttractorType);
    }
}
