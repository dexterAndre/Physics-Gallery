using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController_InputSlider : GUIController
{
    public override void ApplyColorPalette(ColorPalette palette)
    {
        SetSliderInputColors(transform.GetChild(1).GetComponent<GUIIncrementSliderInput>(), palette);
    }
}
