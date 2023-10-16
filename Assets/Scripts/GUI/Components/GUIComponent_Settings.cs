using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class GUIComponent_Settings : GUIComponent
{
    [SerializeField] private GUIFlipToggle controllerGUISize;
    [SerializeField] private GUIFlipToggle controllerDimension;
    [SerializeField] private Toggle controllerShowBounds;
    [SerializeField] private TMP_Dropdown controllerBoundsType;
    [SerializeField] private TMP_Dropdown controllerEdgeResponse;
    [SerializeField] private Toggle controllerShowConstruction;
    [SerializeField] private GUIIncrementSliderInput controllerTotalDuration;
    [SerializeField] private GUIIncrementSliderInput controllerStepDuration;

    public bool IsGUISmall { get { return controllerGUISize.IsFlippedLeft(); } }
    public bool Is2D { get { return controllerGUISize.IsFlippedLeft(); } }
    public bool ShowBounds { get { return controllerShowBounds.isOn; } }
    public BoundsType BoundingType { get { return BoundsType.Cube; } }  // TODO: Implement this
    public EdgeResponse EdgeBehavior { get { return (EdgeResponse)controllerEdgeResponse.value; } }
    public bool ShowConstruction { get { return controllerShowConstruction.isOn; } }
    public float TotalDuration { get { return controllerTotalDuration.SliderValue; } }
    public float StepDuration { get { return controllerStepDuration.SliderValue; } }
}
