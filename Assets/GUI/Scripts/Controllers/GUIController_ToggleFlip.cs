using UnityEngine;
using UnityEngine.UI;

public class GUIController_ToggleFlip : GUIController_Toggle
{
    [SerializeField] private Image toggleGraphicOn;
    [SerializeField] private Image toggleGraphicOff;

    protected override void ToggleStateVisuals(bool isOn)
    {
        base.ToggleStateVisuals(isOn);
        toggle.targetGraphic = isOn ? toggleGraphicOn : toggleGraphicOff;
    }
}
