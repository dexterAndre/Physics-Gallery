using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIComponent_AnimationJitter : GUIComponent
{
    [SerializeField] private GUIIncrementSliderInput controllerJitterSpeed;
    [SerializeField] private Toggle controllerNormalizeJitter;
    [SerializeField] private Toggle controllerRelativeSpeed;

    public float JitterSpeed { get { return controllerJitterSpeed.SliderValue; } }
    public bool NormalizeJitter { get { return controllerNormalizeJitter.isOn; } }
    public bool RelativeSpeed { get { return controllerRelativeSpeed.isOn; } }

    public void UpdateJitterSpeed()
    {
        ((PointBehavior_AnimationJitter)behavior).JitterSpeed = JitterSpeed;
    }
    public void UpdateNormalizeJitter()
    {
        ((PointBehavior_AnimationJitter)behavior).NormalizeJitter = NormalizeJitter;
    }
    public void UpdateRelativeSpeed()
    {
        ((PointBehavior_AnimationJitter)behavior).RelativeSpeed = RelativeSpeed;
    }
}
